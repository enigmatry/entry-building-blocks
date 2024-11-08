using System.Data;
using System.Diagnostics.CodeAnalysis;
using Enigmatry.Entry.Core.Cqrs;
using Enigmatry.Entry.Core.Data;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.EntityFramework.MediatR;

[SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types",
    Justification = "We need to specify the DbContext type, simplifying this would complicate the design")]
[UsedImplicitly]
public abstract class SaveChangesBehavior<TDbContext, TRequest, TResponse>(
    TDbContext dbContext,
    IUnitOfWork unitOfWork,
    ILogger<SaveChangesBehavior<TDbContext, TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest where TDbContext : BaseDbContext
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = default(TResponse);
        var typeName = request.GetGenericTypeName();

        try
        {
            var (skipSaveChanges, reason) = SkipSaveChanges(request);
            if (skipSaveChanges)
            {
                logger.LogDebug("Skipping SaveChanges for {CommandName}, Reason: {Reason}", typeName, reason);
                return await next();
            }

            // only for the transactional commands we begin the transaction
            if (RequiresTransaction(request))
            {
                var strategy = dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction =
                        await dbContext.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
                    var transactionId = transaction.TransactionId;
                    using (logger.BeginScope(
                               new List<KeyValuePair<string, object>> { new("TransactionContext", transactionId) }))
                    {
                        logger.LogDebug("Begin transaction {TransactionId} for {CommandName}", transactionId, typeName);

                        response = await next();

                        logger.LogDebug("Commit transaction {TransactionId} for {CommandName}", transactionId, typeName);

                        await unitOfWork.SaveChangesAsync(cancellationToken);
                        await dbContext.CommitTransactionAsync(transaction, cancellationToken);
                    }
                });
            }
            else
            {
                response = await next();

                logger.LogDebug("Saving changes without transaction for {CommandName}", typeName);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return response!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error Handling transaction for {CommandName}", typeName);
            throw;
        }
    }

    private static bool RequiresTransaction(TRequest request)
    {
        if (request is IBaseCommand command)
        {
            return command.TransactionBehavior == TransactionBehavior.RequiresDbTransaction;
        }

        return true;
    }

    private (bool doSkip, string reason) SkipSaveChanges(TRequest request)
    {
        if (dbContext.HasActiveTransaction)
        {
            return (true, "Transaction is already active");
        }

        return request.IsQuery() ? (true, "Request is a query") : (false, string.Empty);
    }
}

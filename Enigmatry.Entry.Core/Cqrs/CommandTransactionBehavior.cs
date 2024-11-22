using System.Data;
using JetBrains.Annotations;

namespace Enigmatry.Entry.Core.Cqrs;

/// <summary>
/// Specifies the transaction behavior of the command.
/// </summary>
/// <param name="RequiresTransaction">Indicator if command requires transaction</param>
/// <param name="IsolationLevel">Transaction Isolation Level</param>
[PublicAPI]
public record CommandTransactionBehavior(bool RequiresTransaction, IsolationLevel IsolationLevel)
{
    /// <summary>
    /// Command will not create or enroll to an existing transaction 
    /// </summary>
    public static CommandTransactionBehavior NoTransaction { get; private set; } = new(false, IsolationLevel.Unspecified);

    /// <summary>
    /// Default transaction behavior - uses transactions with IsolationLevel.ReadCommited
    /// </summary>
    public static CommandTransactionBehavior Default { get; private set; } =
        TransactionWith(IsolationLevel.ReadCommitted);

    /// <summary>
    /// Uses transaction with custom isolation level
    /// </summary>
    /// <param name="isolationLevel">Isolation level</param>
    /// <returns>Transaction behavior to use</returns>
    public static CommandTransactionBehavior TransactionWith(IsolationLevel isolationLevel) => new(true, isolationLevel);
};

using MediatR;

namespace Enigmatry.Entry.Core.Cqrs;

/// <summary>
/// Base interface for all the commands.
/// </summary>
public interface IBaseCommand
{
    /// <summary>
    /// Transaction behavior of the command. 
    /// </summary>
    public CommandTransactionBehavior TransactionBehavior => CommandTransactionBehavior.Default;
}

/// <summary>
/// Marks a class as a command.
/// </summary>
public interface ICommand : IBaseCommand, IRequest;

/// <summary>
/// Marks a class as a command that returns a response.
/// </summary>
/// <typeparam name="TResponse">Response</typeparam>
public interface ICommand<out TResponse> : IBaseCommand, IRequest<TResponse>;

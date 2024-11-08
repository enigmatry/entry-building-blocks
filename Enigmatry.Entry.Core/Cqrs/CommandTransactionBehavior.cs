namespace Enigmatry.Entry.Core.Cqrs;

public enum CommandTransactionBehavior
{
    /// <summary>
    /// Indicates that the command requires a database transaction.
    /// </summary>
    RequiresDbTransaction,
}

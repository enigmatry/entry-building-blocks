namespace Enigmatry.Entry.Core.Cqrs;

public enum TransactionBehavior
{
    /// <summary>
    /// Indicates that the command requires a database transaction.
    /// </summary>
    RequiresDbTransaction,
}

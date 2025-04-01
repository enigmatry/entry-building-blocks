namespace Enigmatry.Entry.Core.Data;

public interface IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public void CancelSaving();
}

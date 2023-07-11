namespace Enigmatry.Entry.AspNetCore.Authorization;

public interface IAuthorizationProvider<in T>
{
    public bool HasAnyPermission(IEnumerable<T> permission);
}

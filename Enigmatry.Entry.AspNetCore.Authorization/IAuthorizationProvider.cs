namespace Enigmatry.Entry.AspNetCore.Authorization;

public interface IAuthorizationProvider<in T> where T : notnull
{
    public bool AuthorizePermissions(IEnumerable<T> permissions);
}

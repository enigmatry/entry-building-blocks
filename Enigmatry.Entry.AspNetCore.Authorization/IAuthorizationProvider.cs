namespace Enigmatry.Entry.AspNetCore.Authorization;

public interface IAuthorizationProvider
{
    public bool HasAnyRole(IEnumerable<string> roles);
    public bool HasAnyPermission(IEnumerable<string> permission);
}

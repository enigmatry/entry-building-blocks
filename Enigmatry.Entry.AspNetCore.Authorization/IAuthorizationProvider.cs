namespace Enigmatry.Entry.AspNetCore.Authorization
{
    public interface IAuthorizationProvider
    {
        public bool HasAnyRole(string[] roles);
        public bool HasAnyPermission(string[] permission);
    }
}

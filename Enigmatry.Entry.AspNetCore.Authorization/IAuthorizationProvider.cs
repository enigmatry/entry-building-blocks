namespace Enigmatry.Entry.AspNetCore.Authorization
{
    public interface IAuthorizationProvider
    {
        public bool HasRole(string roleName);
        public bool HasPermission(string permission);
    }
}

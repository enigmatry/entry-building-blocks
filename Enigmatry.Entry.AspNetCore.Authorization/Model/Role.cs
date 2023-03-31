namespace Enigmatry.Entry.AspNetCore.Authorization.Model
{
    public record Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<Permission> Permissions { get; set; } = new List<Permission>();
    }
}

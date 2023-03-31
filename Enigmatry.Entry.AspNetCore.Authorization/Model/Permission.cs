namespace Enigmatry.Entry.AspNetCore.Authorization.Model
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public record Permission
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        public int Id { get; set; }
        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;

        // How do make Actions and Entities extensible?
    }
}

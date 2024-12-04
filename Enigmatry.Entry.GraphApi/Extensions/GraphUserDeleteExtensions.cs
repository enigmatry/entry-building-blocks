using JetBrains.Annotations;
using Microsoft.Graph;
using System.Threading.Tasks;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Extensions;

[PublicAPI]
public static class GraphUserDeleteExtensions
{
    /// <summary>
    /// Removes user from B2C directory.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="issuerAssignedId">The id of the user to retrieve. For local accounts this is emailAddress.</param>
    /// <param name="issuer">Issuer of the identity. For local accounts this property is the local AD tenant default domain name.</param>
    /// <returns><see cref="GraphUser"/> or null if user could not be deleted.</returns>
    public static async Task<GraphUser?> RemoveUser(this GraphServiceClient graph, string issuerAssignedId, string issuer)
    {
        var user = await graph.GetUserByIssuerAssignedId(issuerAssignedId, issuer);
        if (user == null)
        {
            return null;
        }

        await graph.RemoveUser(user.Id);
        await graph.Users[user.Id].DeleteAsync();
        return user;
    }

    /// <summary>
    /// Removes user from B2C directory.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">The id of the user to retrieve.</param>
    public static async Task RemoveUser(this GraphServiceClient graph, string? userId) => await graph.Users[userId].DeleteAsync();
}

using JetBrains.Annotations;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Threading.Tasks;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Extensions;

[PublicAPI]
public static class GraphUserCreateExtensions
{
    /// <summary>
    /// Adds the user to the B2C directory.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="displayName">Display name of the user.</param>
    /// <param name="identity">Identities that can be used to sign in to this user account.</param>
    /// <param name="passwordProfile">Password profile for the user. The profile contains the user's password.</param>
    /// <param name="passwordPolicies">Password policies for the user.</param>
    /// <returns><see cref="GraphUser"/> or null if user could not be added.</returns>
    public static async Task<GraphUser?> AddUser(this GraphServiceClient graph, string displayName, ObjectIdentity identity,
        PasswordProfile passwordProfile, string passwordPolicies)
    {
        var user = new GraphUser
        {
            DisplayName = displayName,
            Identities = [identity],
            PasswordProfile = passwordProfile,
            PasswordPolicies = passwordPolicies
        };
        return await graph.AddUser(user);
    }

    /// <summary>
    /// Adds the user to the B2C directory.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="user">User to create</param>
    /// <returns><see cref="GraphUser"/> or null if user could not be added.</returns>
    public static async Task<GraphUser?> AddUser(this GraphServiceClient graph, GraphUser user) => await graph.Users.PostAsync(user);
}

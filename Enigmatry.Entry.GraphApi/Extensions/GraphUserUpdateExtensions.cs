using Enigmatry.Entry.GraphApi.Models;
using JetBrains.Annotations;
using Microsoft.Graph;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Extensions;

[PublicAPI]
public static class GraphUserUpdateExtensions
{
    /// <summary>
    /// Update user.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userDetails">Changeable user details.</param>
    /// <returns><see cref="GraphUser"/> or null if user could not be modified.</returns>
    public static async Task<GraphUser?> UpdateUser(this GraphServiceClient graph, UserDetails userDetails)
    {
        var user = new GraphUser
        {
            PasswordProfile = userDetails.PasswordProfile
        };

        if (userDetails.DisplayName != null)
        {
            user.DisplayName = userDetails.DisplayName;
        }

        if (userDetails.PasswordPolicies != null)
        {
            user.PasswordPolicies = userDetails.PasswordPolicies;
        }

        if (userDetails.Identity != null)
        {
            user.Identities = [userDetails.Identity];
        }

        return await Update(graph, userDetails.UserId, user);
    }

    /// <summary>
    /// Update user sign in email address.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="user">Given graph user.</param>
    /// <param name="oldEmailAddress">User's old email address.</param>
    /// <param name="newEmailAddress">Desired, new email address of user.</param>
    /// <returns><see cref="GraphUser"/> or null if user could not be modified.</returns>
    public static async Task<GraphUser?> UpdateUserSignInEmailAddress(this GraphServiceClient graph, GraphUser user,
        string oldEmailAddress, string newEmailAddress)
    {
        var identity = user.Identities?.SingleOrDefault(
            identity => identity.SignInType == "emailAddress" && identity.IssuerAssignedId == oldEmailAddress);

        if (identity == null)
        {
            return user;
        }

        identity.IssuerAssignedId = newEmailAddress;
        return await Update(graph, user.Id, user);
    }

    private static async Task<GraphUser?> Update(BaseGraphServiceClient graph, string? id, GraphUser user) => await graph.Users[id].PatchAsync(user);
}

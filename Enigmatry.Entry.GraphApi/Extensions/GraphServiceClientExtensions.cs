using Microsoft.Graph;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GraphUser = Microsoft.Graph.User;

namespace Enigmatry.Entry.GraphApi.Extensions;

public static class GraphServiceClientExtensions
{
    /// <summary>
    /// Retrieve a user by id.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">The id of the user to retrieve.</param>
    /// <param name="selectExpression">Lambda expression tree that selects the properties of the returned <see cref="GraphUser"/></param>
    /// <returns><see cref="GraphUser"/></returns>
    public static async Task<GraphUser> GetUserById(this GraphServiceClient graph, string userId,
        Expression<Func<GraphUser, object>> selectExpression) =>
        await graph
            .Users[userId]
            .Request()
            .Select(selectExpression)
            .GetAsync();

    /// <summary>
    /// Retrieve a user by id.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">The id of the user to retrieve.</param>
    /// <returns><see cref="GraphUser"/></returns>
    public static async Task<GraphUser> GetUserById(this GraphServiceClient graph, string userId) =>
        await GetUserById(graph, userId, SelectExpression());

    /// <summary>
    /// Retrieve a user by issuer assigned id.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="issuerAssignedId">The issuerAssignedId of the user to retrieve. For local accounts this is emailAddress.</param>
    /// <param name="issuer">Issuer of the identity. For local accounts this property is the local AD tenant default domain name</param>
    /// <returns><see cref="GraphUser"/></returns>
    public static async Task<GraphUser?> GetUserByIssuerAssignedId(this GraphServiceClient graph, string issuerAssignedId, string issuer)
    {
        var collectionPage = await graph.Users
            .Request()
            .Filter($"identities/any(c:c/issuerAssignedId eq '{issuerAssignedId}' and c/issuer eq '{issuer}')")
            .Select(SelectExpression())
            .GetAsync();

        return collectionPage.SingleOrDefault();
    }

    /// <summary>
    /// Retrieve paged (100) AD users with a default set of properties.
    /// </summary>
    /// <param name="graphClient">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="pageSize">Number of users per page.</param>
    /// <returns><see cref="IGraphServiceUsersCollectionPage"/> collection of <see cref="GraphUser"/>.</returns>
    public static async Task<IGraphServiceUsersCollectionPage> GetUsers(this GraphServiceClient graphClient, int pageSize = 100) =>
        await graphClient.GetUsers(SelectExpression(), pageSize);

    /// <summary>
    /// Retrieve paged (100) AD users.
    /// </summary>
    /// <param name="graphClient">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="pageSize">Number of users per page.</param>
    /// <param name="selectExpression">Lambda expression tree that selects the properties of the returned <see cref="GraphUser"/></param>
    /// <returns><see cref="IGraphServiceUsersCollectionPage"/> collection of <see cref="GraphUser"/>.</returns>
    public static async Task<IGraphServiceUsersCollectionPage> GetUsers(this GraphServiceClient graphClient, Expression<Func<GraphUser, object>> selectExpression, int pageSize = 100) =>
        await graphClient
            .Users
            .Request()
            .Top(pageSize)
            .Select(selectExpression)
            .GetAsync();

    /// <summary>
    /// Search users by display name or email.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="query">User to search for.</param>
    /// <returns><see cref="IGraphServiceUsersCollectionPage"/> collection of <see cref="GraphUser"/>.</returns>
    public static async Task<IGraphServiceUsersCollectionPage> SearchUsers(this GraphServiceClient graph, string query) =>
        await graph
            .Users
            .Request()
            .Filter($"startswith(displayName, '{query}') or startswith(givenName, '{query}') or startswith(surname, '{query}') or startswith(mail, '{query}') or startswith(userPrincipalName, '{query}')")
            .Select(SelectExpression())
            .GetAsync();

    /// <summary>
    /// Helper to get the photo of a particular user.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">UserID.</param>
    /// <returns>Stream with user photo or null.</returns>
    public static async Task<Stream> GetUserPhoto(this GraphServiceClient graph, string userId) =>
        await GetUserPhoto(graph.Users[userId]);

    /// <summary>
    /// Helper to get the photo of a current user.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <returns>Stream with user photo or null.</returns>
    public static async Task<Stream> GetCurrentUserPhoto(this GraphServiceClient graph) =>
        await GetUserPhoto(graph.Me);

    private static Task<Stream> GetUserPhoto(IUserRequestBuilder userBuilder) =>
        userBuilder.Photo
            .Content
            .Request()
            .GetAsync();

    /// <summary>
    /// Adds the user to the B2C directory.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="displayName">Display name of the user.</param>
    /// <param name="identity">Identities that can be used to sign in to this user account.</param>
    /// <param name="passwordProfile">Password profile for the user. The profile contains the user's password.</param>
    /// <param name="passwordPolicies">Password policies for the user.</param>
    /// <returns></returns>
    public static async Task<GraphUser> AddUser(
        this GraphServiceClient graph,
        string displayName,
        ObjectIdentity identity,
        PasswordProfile passwordProfile,
        string passwordPolicies)
    {
        var user = new GraphUser { DisplayName = displayName, Identities = new[] { identity }, PasswordProfile = passwordProfile, PasswordPolicies = passwordPolicies };
        return await graph.Users.Request().AddAsync(user);
    }

    /// <summary>
    /// Adds the user to the B2C directory.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="user">User to create</param>
    /// <returns></returns>
    public static async Task<GraphUser> AddUser(
        this GraphServiceClient graph,
        GraphUser user) =>
        await graph.Users.Request().AddAsync(user);

    /// <summary>
    /// Update user.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">User id.</param>
    /// <param name="displayName">Display name of the user.</param>
    /// <param name="identity">Identities that can be used to sign in to this user account.</param>
    /// <param name="passwordProfile">Password profile for the user. The profile contains the user's password.</param>
    /// <param name="passwordPolicies">Password policies for the user.</param>
    /// <returns></returns>
    public static async Task<GraphUser> UpdateUser(
        this GraphServiceClient graph,
        string userId,
        string displayName,
        ObjectIdentity identity,
        PasswordProfile passwordProfile,
        string passwordPolicies)
    {
        var user = new GraphUser { DisplayName = displayName, Identities = new[] { identity }, PasswordProfile = passwordProfile, PasswordPolicies = passwordPolicies };
        return await graph.Users[userId].Request().UpdateAsync(user);
    }

    /// <summary>
    /// Update user.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">User id.</param>
    /// <param name="displayName">Display name of the user.</param>
    /// <param name="passwordProfile">Password profile for the user. The profile contains the user's password.</param>
    /// <returns></returns>
    public static async Task<GraphUser> UpdateUser(
        this GraphServiceClient graph,
        string userId,
        string displayName,
        PasswordProfile passwordProfile)
    {
        var user = new GraphUser { DisplayName = displayName, PasswordProfile = passwordProfile };
        return await graph.Users[userId].Request().UpdateAsync(user);
    }

    /// <summary>
    /// Update user password.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">User id.</param>
    /// <param name="passwordProfile">Password profile for the user. The profile contains the user's password.</param>
    /// <param name="passwordPolicies">Specifies password policies for the user, for example: DisablePasswordExpiration, DisableStrongPassword.</param>
    /// <returns></returns>
    public static async Task<GraphUser> UpdateUserPassword(
        this GraphServiceClient graph,
        string userId,
        PasswordProfile passwordProfile,
        string passwordPolicies)
    {
        var user = new GraphUser { PasswordProfile = passwordProfile, PasswordPolicies = passwordPolicies };
        return await graph.Users[userId].Request().UpdateAsync(user);
    }

    /// <summary>
    /// Update user sign in email address.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="user"></param>
    /// <param name="oldEmailAddress"></param>
    /// <param name="newEmailAddress"></param>
    /// <returns></returns>
    public static async Task<GraphUser> UpdateUserSignInEmailAddress(
        this GraphServiceClient graph,
        GraphUser user,
        string oldEmailAddress,
        string newEmailAddress)
    {
        var identity = user.Identities.SingleOrDefault(
            identity => identity.SignInType == "emailAddress" && identity.IssuerAssignedId == oldEmailAddress);

        if (identity == null)
        {
            return user;
        }

        identity.IssuerAssignedId = newEmailAddress;

        return await graph.Users[user.Id]
            .Request()
            .UpdateAsync(user);
    }

    /// <summary>
    /// Removes user from B2C directory.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="issuerAssignedId">The id of the user to retrieve. For local accounts this is emailAddress.</param>
    /// <param name="issuer">Issuer of the identity. For local accounts this property is the local AD tenant default domain name.</param>
    /// <returns></returns>
    public static async Task<GraphUser?> RemoveUser(this GraphServiceClient graph, string issuerAssignedId, string issuer)
    {
        var user = await graph.GetUserByIssuerAssignedId(issuerAssignedId, issuer);
        if (user != null)
        {
            await graph.Users[user.Id].Request().DeleteAsync();
        }
        return user;
    }

    /// <summary>
    /// Tries removal of user from B2C directory.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="issuerAssignedId">The id of the user to retrieve. For local accounts this is emailAddress.</param>
    /// <param name="issuer">Issuer of the identity. For local accounts this property is the local AD tenant default domain name.</param>
    /// <returns>User data of deleted user.</returns>
    public static async Task<GraphUser?> TryRemovingUser(this GraphServiceClient graph, string issuerAssignedId, string issuer)
    {
        var user = await graph.GetUserByIssuerAssignedId(issuerAssignedId, issuer);
        if (user != null)
        {
            await graph.RemoveUser(user.Id);
        }
        return user;
    }

    /// <summary>
    /// Removes user from B2C directory.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">The id of the user to retrieve.</param>
    public static async Task RemoveUser(this GraphServiceClient graph, string userId) => await graph.Users[userId].Request().DeleteAsync();

    private static Expression<Func<GraphUser, object>> SelectExpression() =>
        graphUser =>
            new
            {
                graphUser.Id,
                graphUser.DisplayName,
                graphUser.GivenName,
                graphUser.Surname,
                graphUser.Identities,
                graphUser.AccountEnabled,
                graphUser.UserPrincipalName,
                graphUser.Mail
            };
}

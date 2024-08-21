using Enigmatry.Entry.Core.Helpers;
using JetBrains.Annotations;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Extensions;

[PublicAPI]
public static class GraphClientSearchExtensions
{
    /// <summary>
    /// Retrieve a user by id.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">The id of the user to retrieve.</param>
    /// <param name="selectExpression">Lambda function that selects the properties of the returned <see cref="GraphUser"/></param>
    /// <returns><see cref="GraphUser"/></returns>
    public static async Task<GraphUser?> GetUserById(this GraphServiceClient graph, string userId, Func<GraphUser, IEnumerable<string>> selectExpression) =>
        await graph.Users[userId].GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select = Adjust(selectExpression);
            });

    /// <summary>
    /// Retrieve a user by id.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">The id of the user to retrieve.</param>
    /// <returns><see cref="GraphUser"/></returns>
    public static async Task<GraphUser?> GetUserById(this GraphServiceClient graph, string userId) =>
        await GetUserById(graph, userId, _ => AdjustedDefaultFields());

    /// <summary>
    /// Retrieve a user by issuer assigned id.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="issuerAssignedId">The issuerAssignedId of the user to retrieve. For local accounts this is emailAddress.</param>
    /// <param name="issuer">Issuer of the identity. For local accounts this property is the local AD tenant default domain name</param>
    /// <returns><see cref="GraphUser"/></returns>
    public static async Task<GraphUser?> GetUserByIssuerAssignedId(this GraphServiceClient graph,
        string issuerAssignedId, string issuer)
    {
        var users = await graph.Users.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = AdjustedDefaultFields();
            requestConfiguration.QueryParameters.Filter =
                $"identities/any(c:c/issuerAssignedId eq '{issuerAssignedId}' and c/issuer eq '{issuer}')";
        });

        return users?.Value?.SingleOrDefault();
    }

    /// <summary>
    /// Retrieve paged (100) AD users with a default set of properties.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="pageSize">Number of users per page.</param>
    /// <returns>Collection of <see cref="GraphUser"/>.</returns>
    public static async Task<IList<GraphUser>> GetUsers(this GraphServiceClient graph, int pageSize = 100) =>
        await graph.GetUsers(_ => AdjustedDefaultFields(), pageSize);

    /// <summary>
    /// Retrieve paged (100) AD users.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="pageSize">Number of users per page.</param>
    /// <param name="selectExpression">Lambda function that selects the properties of the returned <see cref="GraphUser"/></param>
    /// <returns>Collection of <see cref="GraphUser"/>.</returns>
    public static async Task<IList<GraphUser>> GetUsers(this GraphServiceClient graph,
        Func<GraphUser, IEnumerable<string>> selectExpression, int pageSize = 100) =>
        Normalize(await graph.Users.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = Adjust(selectExpression);
            requestConfiguration.QueryParameters.Top = pageSize;
        }));

    /// <summary>
    /// Search users by display name or email.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="query">User to search for.</param>
    /// <returns>Collection of <see cref="GraphUser"/>.</returns>
    public static async Task<IList<GraphUser>> SearchUsers(this GraphServiceClient graph, string query) =>
        Normalize(await graph.Users.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = AdjustedDefaultFields();
            requestConfiguration.QueryParameters.Filter = $"startswith(displayName, '{query}') or startswith(givenName, '{query}') or startswith(surname, '{query}') or startswith(mail, '{query}') or startswith(userPrincipalName, '{query}')";
        }));

    private static GraphUser User() => new();

    private static IList<GraphUser> Normalize(UserCollectionResponse? response) => response?.Value ?? [];
    private static string[] AdjustedDefaultFields() => Adjust(DefaultFields);
    private static string[] Adjust(Func<GraphUser, IEnumerable<string>> desiredPropertiesOf) =>
        desiredPropertiesOf(User())
            .Select(property => property.ToCamelCase())
            .ToArray();

    [SuppressMessage("ReSharper", "UnusedParameter.Local",
        Justification = "False positive")]
    private static IEnumerable<string> DefaultFields(GraphUser user) =>
    [
        nameof(user.Id), nameof(user.DisplayName), nameof(user.GivenName), nameof(user.Surname),
        nameof(user.Identities), nameof(user.AccountEnabled), nameof(user.UserPrincipalName), nameof(user.Mail)
    ];
}

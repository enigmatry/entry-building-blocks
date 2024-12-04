using JetBrains.Annotations;
using Microsoft.Graph;
using System.IO;
using System.Threading.Tasks;

namespace Enigmatry.Entry.GraphApi.Extensions;

[PublicAPI]
public static class GraphClientPhotoExtensions
{
    /// <summary>
    /// Helper to get the photo of a particular user.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <param name="userId">UserID.</param>
    /// <returns>Stream with user photo or null.</returns>
    public static async Task<Stream?> GetUserPhoto(this GraphServiceClient graph, string userId) =>
        await graph.Users[userId].Photo.Content.GetAsync();

    /// <summary>
    /// Helper to get the photo of a current user.
    /// </summary>
    /// <param name="graph">Instance of the <see cref="GraphServiceClient"/>.</param>
    /// <returns>Stream with user photo or null.</returns>
    public static async Task<Stream?> GetCurrentUserPhoto(this GraphServiceClient graph) =>
        await graph.Me.Photo.Content.GetAsync();
}

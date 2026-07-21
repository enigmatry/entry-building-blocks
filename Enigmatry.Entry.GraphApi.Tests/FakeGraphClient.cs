using FakeItEasy;
using FakeItEasy.Core;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Serialization.Json;
using System.Text.Json;
using GraphUser = Microsoft.Graph.Models.User;

namespace Enigmatry.Entry.GraphApi.Tests;

/// <summary>
/// A <see cref="GraphServiceClient"/> over a faked <see cref="IRequestAdapter"/> that records every
/// <see cref="RequestInformation"/> the fluent API builds and returns the configured responses.
/// </summary>
internal sealed class FakeGraphClient : IDisposable
{
    public GraphServiceClient Client { get; }
    public List<RequestInformation> Requests { get; } = [];

    internal FakeGraphClient(GraphUser? userResponse, UserCollectionResponse? usersResponse, Stream? photoResponse)
    {
        var adapter = A.Fake<IRequestAdapter>();

        A.CallTo(() => adapter.SerializationWriterFactory).Returns(new JsonSerializationWriterFactory());

        A.CallTo(() => adapter.SendAsync(A<RequestInformation>._, A<ParsableFactory<GraphUser>>._,
                A<Dictionary<string, ParsableFactory<IParsable>>>._, A<CancellationToken>._))
            .ReturnsLazily(call => Capture(call, userResponse));

        A.CallTo(() => adapter.SendAsync(A<RequestInformation>._, A<ParsableFactory<UserCollectionResponse>>._,
                A<Dictionary<string, ParsableFactory<IParsable>>>._, A<CancellationToken>._))
            .ReturnsLazily(call => Capture(call, usersResponse));

        A.CallTo(() => adapter.SendPrimitiveAsync<Stream>(A<RequestInformation>._,
                A<Dictionary<string, ParsableFactory<IParsable>>>._, A<CancellationToken>._))
            .ReturnsLazily(call => Capture(call, photoResponse));

        A.CallTo(() => adapter.SendNoContentAsync(A<RequestInformation>._,
                A<Dictionary<string, ParsableFactory<IParsable>>>._, A<CancellationToken>._))
            .ReturnsLazily(call =>
            {
                Requests.Add((RequestInformation)call.Arguments[0]!);
                return Task.CompletedTask;
            });

        Client = new GraphServiceClient(adapter);
    }

    public RequestInformation SingleRequest => Requests.Single();

    public object SingleRequestSnapshot() => Snapshot(SingleRequest);

    public IEnumerable<object> RequestsSnapshot() => Requests.Select(Snapshot);

    public void Dispose() => Client.Dispose();

    private Task<T?> Capture<T>(IFakeObjectCall call, T? response)
    {
        Requests.Add((RequestInformation)call.Arguments[0]!);
        return Task.FromResult(response);
    }

    private static object Snapshot(RequestInformation request) => new
    {
        Method = request.HttpMethod,
        Url = request.URI.GetLeftPart(UriPartial.Path),
        Query = request.QueryParameters,
        Body = ReadBody(request)
    };

    private static string? ReadBody(RequestInformation request)
    {
        if (request.Content == null || request.Content.Length == 0)
        {
            return null;
        }

        using var document = JsonDocument.Parse(request.Content);
        return JsonSerializer.Serialize(document.RootElement,
            new JsonSerializerOptions { WriteIndented = true, NewLine = "\n" });
    }
}

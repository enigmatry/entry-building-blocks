using FakeItEasy;
using FakeItEasy.Core;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Serialization.Json;
using System.Text.Json;

namespace Enigmatry.Entry.GraphApi.Tests;

/// <summary>
/// A <see cref="GraphServiceClient"/> over a faked <see cref="IRequestAdapter"/> that records every
/// <see cref="RequestInformation"/> the fluent API builds and returns canned responses.
/// </summary>
internal sealed class FakeGraphClient
{
    private readonly IRequestAdapter _adapter = A.Fake<IRequestAdapter>();

    public GraphServiceClient Client { get; }
    public List<RequestInformation> Requests { get; } = [];

    public User? UserResponse { get; set; }
    public UserCollectionResponse? UsersResponse { get; set; }
    public Stream? StreamResponse { get; set; }

    public FakeGraphClient()
    {
        A.CallTo(() => _adapter.SerializationWriterFactory).Returns(new JsonSerializationWriterFactory());

        A.CallTo(() => _adapter.SendAsync(A<RequestInformation>._, A<ParsableFactory<User>>._,
                A<Dictionary<string, ParsableFactory<IParsable>>>._, A<CancellationToken>._))
            .ReturnsLazily(call => Capture(call, UserResponse));

        A.CallTo(() => _adapter.SendAsync(A<RequestInformation>._, A<ParsableFactory<UserCollectionResponse>>._,
                A<Dictionary<string, ParsableFactory<IParsable>>>._, A<CancellationToken>._))
            .ReturnsLazily(call => Capture(call, UsersResponse));

        A.CallTo(() => _adapter.SendPrimitiveAsync<Stream>(A<RequestInformation>._,
                A<Dictionary<string, ParsableFactory<IParsable>>>._, A<CancellationToken>._))
            .ReturnsLazily(call => Capture(call, StreamResponse));

        A.CallTo(() => _adapter.SendNoContentAsync(A<RequestInformation>._,
                A<Dictionary<string, ParsableFactory<IParsable>>>._, A<CancellationToken>._))
            .ReturnsLazily(call =>
            {
                Requests.Add((RequestInformation)call.Arguments[0]!);
                return Task.CompletedTask;
            });

        Client = new GraphServiceClient(_adapter);
    }

    public RequestInformation SingleRequest => Requests.Single();

    public JsonElement SingleRequestJsonBody()
    {
        using var document = JsonDocument.Parse(SingleRequest.Content);
        return document.RootElement.Clone();
    }

    private Task<T?> Capture<T>(IFakeObjectCall call, T? response)
    {
        Requests.Add((RequestInformation)call.Arguments[0]!);
        return Task.FromResult(response);
    }
}

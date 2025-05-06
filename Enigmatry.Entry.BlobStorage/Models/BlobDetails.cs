using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage.Models;

[PublicAPI]
public record BlobDetails(string Name, IDictionary<string, string> Metadata);

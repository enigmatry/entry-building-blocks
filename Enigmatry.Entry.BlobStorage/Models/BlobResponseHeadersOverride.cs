using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage.Models;

[PublicAPI]
public record BlobResponseHeadersOverride(
    string? CacheControl,
    ContentDisposition? ContentDisposition,
    string? ContentEncoding,
    string? ContentLanguage,
    string? ContentType);

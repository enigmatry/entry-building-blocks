using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage.Models;

[PublicAPI]
public class BlobResponseHeadersOverrideBuilder
{
    private string? _cacheControl;
    private ContentDisposition? _contentDisposition;
    private string? _contentEncoding;
    private string? _contentLanguage;
    private string? _contentType;

    public BlobResponseHeadersOverrideBuilder WithCacheControl(string? cacheControl)
    {
        _cacheControl = cacheControl;
        return this;
    }

    public BlobResponseHeadersOverrideBuilder WithContentDisposition(ContentDisposition contentDisposition)
    {
        _contentDisposition = contentDisposition;
        return this;
    }

    public BlobResponseHeadersOverrideBuilder WithContentEncoding(string? contentEncoding)
    {
        _contentEncoding = contentEncoding;
        return this;
    }

    public BlobResponseHeadersOverrideBuilder WithContentLanguage(string? contentLanguage)
    {
        _contentLanguage = contentLanguage;
        return this;
    }

    public BlobResponseHeadersOverrideBuilder WithContentType(string contentType)
    {
        _contentType = contentType;
        return this;
    }

    public static implicit operator BlobResponseHeadersOverride(BlobResponseHeadersOverrideBuilder builder) =>
        ToBlobResponseHeadersOverride(builder);

    public static BlobResponseHeadersOverride ToBlobResponseHeadersOverride(BlobResponseHeadersOverrideBuilder builder) =>
        builder.Build();

    public BlobResponseHeadersOverride Build() =>
        new(_cacheControl, _contentDisposition, _contentEncoding, _contentLanguage, _contentType);
}

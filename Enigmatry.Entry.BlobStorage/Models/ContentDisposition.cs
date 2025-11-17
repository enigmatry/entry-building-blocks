using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Enigmatry.Entry.Core.Helpers;
using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage.Models;

[PublicAPI]
public record ContentDisposition(string FileName, ContentDispositionType Type)
{
    public override string ToString()
    {
        if (FileName.HasNoContent())
        {
            return string.Empty;
        }

        var contentDisposition = new ContentDispositionHeaderValue(Type.GetDisplayName()) { FileName = GetSanitizedFileName() };
        return contentDisposition.ToString();
    }

    internal static ContentDisposition? Parse(string? value)
    {
        if (value.HasNoContent() || !ContentDispositionHeaderValue.TryParse(value!, out var contentDisposition))
        {
            return null;
        }

        var fileName = contentDisposition.FileName!.Trim('\"');
        var type = contentDisposition.DispositionType == ContentDispositionType.Attachment.GetDisplayName()
            ? ContentDispositionType.Attachment
            : ContentDispositionType.Inline;
        return new ContentDisposition(fileName, type);
    }

    private string GetSanitizedFileName()
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var pattern = $"[{string.Join(string.Empty, invalidChars.Select(c => Regex.Escape(c.ToString())))}]";
        return Regex.Replace(FileName, pattern, "_");
    }
}

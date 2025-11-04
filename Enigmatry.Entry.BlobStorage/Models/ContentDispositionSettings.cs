using System.Text.RegularExpressions;
using Enigmatry.Entry.Core.Helpers;
using JetBrains.Annotations;

namespace Enigmatry.Entry.BlobStorage.Models;

[PublicAPI]
public record ContentDispositionSettings(string FileName, ContentDispositionType Type)
{
    private const string AttachmentTypePrefix = "attachment; ";
    private const string InlineTypePrefix = "inline; ";
    private const string FileNamePrefix = "filename=\"";
    private const string FileNameSuffix = "\"";

    internal string GetValue()
    {
        if (FileName.HasNoContent())
        {
            return string.Empty;
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var pattern = $"[{string.Join(string.Empty, invalidChars.Select(c => Regex.Escape(c.ToString())))}]";
        var sanitizedFileName = Regex.Replace(FileName, pattern, "_");
        var valuePrefix = GetValuePrefix(Type);
        return $"{valuePrefix}{sanitizedFileName}{FileNameSuffix}";
    }

    internal static ContentDispositionSettings? Parse(string? value)
    {
        if (value.HasNoContent())
        {
            return null;
        }

        ContentDispositionType? type = null;
        if (value!.StartsWith(AttachmentTypePrefix, StringComparison.InvariantCulture))
        {
            type = ContentDispositionType.Attachment;
        }
        else if (value.StartsWith(InlineTypePrefix, StringComparison.InvariantCulture))
        {
            type = ContentDispositionType.Inline;
        }

        if (!type.HasValue)
        {
            return null;
        }

        var valuePrefix = GetValuePrefix(type.Value);
        var fileName = value[valuePrefix.Length..^FileNameSuffix.Length];
        return new ContentDispositionSettings(fileName, type.Value);
    }

    private static string GetTypePrefix(ContentDispositionType type) =>
        type switch
        {
            ContentDispositionType.Attachment => AttachmentTypePrefix,
            ContentDispositionType.Inline => InlineTypePrefix,
            _ => throw new InvalidOperationException("Unknown content disposition type")
        };

    private static string GetValuePrefix(ContentDispositionType type) =>
        $"{GetTypePrefix(type)}{FileNamePrefix}";
}

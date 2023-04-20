using JetBrains.Annotations;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Enigmatry.Entry.Core.Images;

[PublicAPI]
public record ImageDataUri
{
    private const string Pattern = @"data:image/(?<type>.+?),(?<data>.+)";
    private readonly string _content;

    public static ImageDataUri CreateFrom(byte[] array, string contentType)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (array.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(array), @"Cannot create data uri without a source for image!");
        }

        if (string.IsNullOrEmpty(contentType))
        {
            throw new ArgumentException(@"Mime type must be valid image type (i.e.image/png)!", nameof(contentType));
        }

        var imageBase64 = Convert.ToBase64String(array);
        return new ImageDataUri($"data:{contentType};base64,{imageBase64}");
    }

    public static ImageDataUri CreateFrom(string content)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        var invalidDataUri = !Regex.Match(content, Pattern, RegexOptions.Compiled).Success;
        if (invalidDataUri)
        {
            throw new ArgumentException("Data uri is invalid!", nameof(content));
        }

        return new ImageDataUri(content);
    }

    private ImageDataUri(string content)
    {
        _content = content;
    }

    public byte[] ToByteArray()
    {
        var data = _content.Split(',').Last();
        return Convert.FromBase64String(data);
    }

    public override string ToString() => _content;
}

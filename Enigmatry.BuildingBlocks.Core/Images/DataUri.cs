using JetBrains.Annotations;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Core.Images
{
    [PublicAPI]
    public record DataUri
    {
        private const string Pattern = @"data:image/(?<type>.+?),(?<data>.+)";
        private readonly string _content;

        public static DataUri CreateFrom(byte[] array, string mimeType)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(array), @"Cannot create data uri without a source for image!");
            }

            if (string.IsNullOrEmpty(mimeType))
            {
                throw new ArgumentException(@"Mime type must be valid image type (i.e.image/png)!", nameof(mimeType));
            }

            var imageBase64 = Convert.ToBase64String(array);
            return new DataUri($"data:{mimeType};base64,{imageBase64}");
        }

        public static DataUri CreateFrom(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var invalidDataUri = !Regex.Match(content, Pattern).Success;
            if (invalidDataUri)
            {
                throw new ArgumentException(nameof(content));
            }

            return new DataUri(content);
        }

        private DataUri(string content)
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
}

using System;

namespace Enigmatry.BuildingBlocks.Tests.Core.Images
{
    internal class StringBytes
    {
        internal string PngUri => "data:image/png;base64," + DataUri;
        internal string DataUri { get; set; } = string.Empty;
        internal byte[] Bytes { get; set; } = Array.Empty<byte>();
    }
}

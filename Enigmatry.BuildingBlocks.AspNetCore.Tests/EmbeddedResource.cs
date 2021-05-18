using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Enigmatry.BuildingBlocks.AspNetCore.Tests
{
    public static class EmbeddedResource
    {
        public static string ReadResourceContent(string namespaceAndFileName)
        {
            try
            {
                using Stream? stream = typeof(EmbeddedResource).GetTypeInfo().Assembly.GetManifestResourceStream(namespaceAndFileName);
                if (stream == null)
                {
                    return String.Empty;
                }
                using var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Failed to read Embedded Resource {namespaceAndFileName}", exception);
            }
        }
    }
}

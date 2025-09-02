using System.Text.RegularExpressions;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Microsoft.Extensions.AI;

namespace Enigmatry.Entry.AzureSearch.Vectors;

public class EmbeddingService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : IEmbeddingService
{
    private static readonly Regex DataUrlImageRegex = new("""<img[^>]*src\s*=\s*["']data:[^"']*["'][^>]*>""", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public async Task<float[]> EmbedText(string inputText)
    {
        // Strip HTML img tags with data URLs, because they can contain large base64 encoded images leading to excessive token counts.
        var cleanedText = DataUrlImageRegex.Replace(inputText, string.Empty);

        var embedding = await embeddingGenerator.GenerateAsync(cleanedText);
        return embedding.Vector.ToArray();
    }
}

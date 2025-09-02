namespace Enigmatry.Entry.AzureSearch.Abstractions;
public interface IEmbeddingService
{
    Task<float[]> EmbedText(string inputText);
}

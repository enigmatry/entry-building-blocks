namespace Enigmatry.Entry.AzureSearch.Abstractions;
public interface IEmbeddingService
{
    float[] EmbedText(string inputText);
}

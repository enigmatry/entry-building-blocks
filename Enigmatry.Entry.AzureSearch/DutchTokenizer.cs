using Azure.Search.Documents.Indexes.Models;

namespace Enigmatry.Entry.AzureSearch;
public static class DutchTokenizer
{
    private const string DescriptionTokenizerName = "description_tokenizer";

    public static LexicalTokenizer Create() =>
        new MicrosoftLanguageTokenizer(DescriptionTokenizerName) { Language = MicrosoftTokenizerLanguage.Dutch };
}

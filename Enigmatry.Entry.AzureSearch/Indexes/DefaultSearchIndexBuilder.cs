using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Enigmatry.Entry.AzureSearch.Abstractions;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.AzureSearch.Indexes;

public class DefaultSearchIndexBuilder<T> : ISearchIndexBuilder<T>
{
    private readonly ISearchIndexNameResolver<T> _indexNameResolver;
    private readonly ILogger<DefaultSearchIndexBuilder<T>> _logger;
    private IEnumerable<LexicalTokenizer>? _tokenizers;
    private IEnumerable<LexicalAnalyzer>? _analyzers;
    private IEnumerable<ScoringProfile>? _scoringProfiles;
    private VectorSearch? _vectorSearch;

    public DefaultSearchIndexBuilder(ISearchIndexNameResolver<T> indexNameResolver, ILogger<DefaultSearchIndexBuilder<T>> logger)
    {
        _indexNameResolver = indexNameResolver;
        _logger = logger;
    }

    public ISearchIndexBuilder<T> With(IEnumerable<LexicalTokenizer> tokenizers)
    {
        _tokenizers = tokenizers;
        return this;
    }

    public ISearchIndexBuilder<T> With(IEnumerable<LexicalAnalyzer> analyzers)
    {
        _analyzers = analyzers;
        return this;
    }

    public ISearchIndexBuilder<T> With(IEnumerable<ScoringProfile> scoringProfiles)
    {
        _scoringProfiles = scoringProfiles;
        return this;
    }

    public ISearchIndexBuilder<T> With(VectorSearch vectorSearch)
    {
        _vectorSearch = vectorSearch;
        return this;
    }

    public virtual SearchIndex Build()
    {
        FieldBuilder fieldBuilder = new();
        var searchFields = fieldBuilder.Build(typeof(T));

        var indexName = _indexNameResolver.ResolveIndexName();
        _logger.LogInformation("Creating index: {IndexName}", indexName);

        var index = new SearchIndex(indexName, searchFields);

        foreach (var tokenizer in _tokenizers ?? Array.Empty<LexicalTokenizer>())
        {
            index.Tokenizers.Add(tokenizer);
        }

        foreach (var analyzer in _analyzers ?? Array.Empty<LexicalAnalyzer>())
        {
            index.Analyzers.Add(analyzer);
        }

        foreach (var scoringProfile in _scoringProfiles ?? Array.Empty<ScoringProfile>())
        {
            index.ScoringProfiles.Add(scoringProfile);
        }

        if (_vectorSearch != null)
        {
            index.VectorSearch = _vectorSearch;
        }

        return index;
    }
}

using Azure.Search.Documents.Indexes;
using JetBrains.Annotations;

namespace Enigmatry.Entry.AzureSearch.Tests.Documents;

[UsedImplicitly]
public class TestDocument
{
    [SearchableField(IsKey = true, IsFilterable = true)]
    public string Id { get; set; } = String.Empty;

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string Name { get; set; } = String.Empty;// Field that can both be searched and filtered

    [SearchableField(IsFilterable = false)]
    public string Description { get; set; } = String.Empty;// Field that can only be searched by the search text, but not filtered
}

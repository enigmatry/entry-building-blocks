using Azure.Search.Documents.Indexes;
using JetBrains.Annotations;

namespace Enigmatry.Entry.AzureSearch.Tests.Documents;

[UsedImplicitly]
public class TestDocument
{
    [SearchableField(IsKey = true, IsFilterable = true)]
    public string Id { get; set; } = String.Empty;

    [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
    public string Name { get; set; } = String.Empty;// Field that can both be searched, filtered and faceted

    [SearchableField(IsFilterable = false)]
    public string Description { get; set; } = String.Empty;// Field that can only be searched by the search text, but not filtered

    [SimpleField(IsFacetable = true)]
    public int Rating { get; set; }

    [SimpleField(IsFacetable = true)]
    public DateTimeOffset CreatedOn { get; set; }
}

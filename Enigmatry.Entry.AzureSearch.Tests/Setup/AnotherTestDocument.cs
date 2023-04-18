using Azure.Search.Documents.Indexes;
using JetBrains.Annotations;

namespace Enigmatry.Entry.AzureSearch.Tests.Setup;

[UsedImplicitly]
public class AnotherTestDocument
{
    [SimpleField(IsKey = true, IsFilterable = true)]
    public string Id { get; set; } = String.Empty;
}

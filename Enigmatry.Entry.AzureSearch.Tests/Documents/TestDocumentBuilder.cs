namespace Enigmatry.Entry.AzureSearch.Tests.Documents;

public class TestDocumentBuilder
{
    private string _id = "12343";
    private string _name = "DocumentName";
    private string _description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

    public TestDocumentBuilder WithId(string value)
    {
        _id = value;
        return this;
    }

    public TestDocumentBuilder WithName(string value)
    {
        _name = value;
        return this;
    }

    public TestDocumentBuilder WithDescription(string value)
    {
        _description = value;
        return this;
    }

    public TestDocument Build() => new() { Id = _id, Name = _name, Description = _description };
}

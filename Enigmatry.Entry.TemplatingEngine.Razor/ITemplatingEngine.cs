namespace Enigmatry.Entry.TemplatingEngine;

public interface ITemplatingEngine
{
    public Task<string> RenderFromFileAsync<T>(string path, T model);
    public Task<string> RenderFromFileAsync<T>(string path, T model, IDictionary<string, object> viewBagDictionary);
}

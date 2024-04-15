namespace Enigmatry.Entry.TemplatingEngine.Liquid
{
    public interface ITemplatingEngine
    {
        Task<string> RenderFromFileAsync<T>(string path, T model);
        Task<string> RenderFromFileAsync<T>(string path, T model, IDictionary<string, object> viewBagDictionary);
    }
}

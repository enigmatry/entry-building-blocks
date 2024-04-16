namespace Enigmatry.Entry.TemplatingEngine.Liquid
{
    public interface ITemplatingEngine
    {
        Task<string> RenderAsync<T>(string path, T model);
        Task<string> RenderAsync<T>(string path, T model, IDictionary<string, object> viewBagDictionary);
    }
}

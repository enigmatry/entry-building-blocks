namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public interface ITemplatingEngine
{
    public Task<string> RenderAsync<T>(string pattern, T model);
}

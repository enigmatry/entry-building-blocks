namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public interface ITemplatingEngine
{
    Task<string> RenderAsync<T>(string pattern, T model);
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.TemplatingEngine
{
    public interface ITemplatingEngine
    {
        Task<string> RenderFromFileAsync<T>(string path, T model);
        Task<string> RenderFromFileAsync<T>(string path, T model, IDictionary<string, object> viewBagDictionary);
    }
}

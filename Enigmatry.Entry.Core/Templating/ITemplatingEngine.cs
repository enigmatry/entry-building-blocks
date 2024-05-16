using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enigmatry.Entry.Core.Templating
{
    public interface ITemplatingEngine
    {
        Task<string> RenderFromFileAsync<T>(string path, T model);
        Task<string> RenderFromFileAsync<T>(string path, T model, IDictionary<string, object> viewBagDictionary);
    }
}

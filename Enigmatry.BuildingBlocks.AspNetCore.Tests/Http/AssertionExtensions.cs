using System.Net.Http;

namespace Enigmatry.BuildingBlocks.AspNetCore.Tests.Http
{
    public static class AssertionExtensions
    {
        public static HttpResponseAssertions Should(this HttpResponseMessage actualValue) => new HttpResponseAssertions(actualValue);
    }
}

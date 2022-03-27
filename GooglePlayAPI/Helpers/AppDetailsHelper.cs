using GooglePlayApi.Models;
using GooglePlayApi.Proto;
using GooglePlayApi.Providers;
using System.Net.Http;
using System.Threading.Tasks;

namespace GooglePlayApi.Helpers
{
    public class AppDetailsHelper
    {
        public AuthData AuthData { get; private set; }
        public HttpClient HttpClient { get; private set; }

        public AppDetailsHelper(AuthData authData, HttpClient httpClient)
        {
            AuthData = authData;
            HttpClient = httpClient;
        }

        public async Task<DetailsResponse> GetAppByPackageName(string packageName)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(HeaderProvider.GetDefaultHeaders(AuthData));

            HttpResponseMessage response = await HttpClient.GetAsync(Constants.URL_DETAILS + $"?doc={packageName}");

            return ResponseWrapper.Parser.ParseFrom(response.Content.ReadAsStream()).Payload.DetailsResponse;
        }
    }
}

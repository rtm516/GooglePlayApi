using Google.Protobuf;
using GooglePlayApi.Models;
using GooglePlayApi.Proto;
using GooglePlayApi.Providers;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async Task<TestingProgramResponse> TestingProgram(string packageName, bool subscribe)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(HeaderProvider.GetDefaultHeaders(AuthData));

            using (MemoryStream memStream = new MemoryStream())
            {
                new TestingProgramRequest()
                {
                    PackageName = packageName,
                    Subscribe = subscribe
                }.WriteTo(memStream);

                memStream.Position = 0;

                StreamContent streamContent = new StreamContent(memStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");

                HttpResponseMessage response = await HttpClient.PostAsync(Constants.URL_TESTING_PROGRAM, streamContent);
                return ResponseWrapper.Parser.ParseFrom(await response.Content.ReadAsByteArrayAsync()).Payload.TestingProgramResponse;
            }
        }
    }
}

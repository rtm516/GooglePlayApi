using GooglePlayApi.Models;
using GooglePlayApi.Proto;
using GooglePlayApi.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GooglePlayApi.Helpers
{
    public class PurchaseHelper
    {
        public AuthData AuthData { get; private set; }
        public HttpClient HttpClient { get; private set; }

        public PurchaseHelper(AuthData authData, HttpClient httpClient)
        {
            AuthData = authData;
            HttpClient = httpClient;
        }

        public async Task<DeliveryResponse> GetDeliveryResponse(string packageName, int versionCode, int offerType=1)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(HeaderProvider.GetDefaultHeaders(AuthData));

            HttpResponseMessage response = await HttpClient.GetAsync(Constants.DELIVERY_URL + $"?ot={offerType}&doc={packageName}&vc={versionCode}");

            return ResponseWrapper.Parser.ParseFrom(response.Content.ReadAsStream()).Payload.DeliveryResponse;
        }
    }
}

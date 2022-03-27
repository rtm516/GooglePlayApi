using GooglePlayApi.Proto;
using GooglePlayApi.Models;
using GooglePlayApi.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Google.Protobuf;

namespace GooglePlayApi
{
    public class GooglePlayApi
    {
        public AuthData AuthData { get; private set; }

        public HttpClient HttpClient { get; private set; }

        public GooglePlayApi(AuthData authData, HttpClient httpClient)
        {
            AuthData = authData;
            HttpClient = httpClient;
        }

        public async Task<string> GenerateGsfId(DeviceInfoProvider deviceInfoProvider)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(HeaderProvider.GetAuthHeaders(AuthData));

            using (MemoryStream memStream = new MemoryStream())
            {
                deviceInfoProvider.GenerateAndroidCheckInRequest().WriteTo(memStream);

                memStream.Position = 0;                

                StreamContent streamContent = new StreamContent(memStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");

                HttpResponseMessage response = await HttpClient.PostAsync(Constants.URL_CHECK_IN, streamContent);
                AndroidCheckinResponse checkInResponse = AndroidCheckinResponse.Parser.ParseFrom(await response.Content.ReadAsByteArrayAsync());

                AuthData.GsfId = string.Format("{0:X}", checkInResponse.AndroidId);
                AuthData.DeviceCheckInConsistencyToken = checkInResponse.DeviceCheckinConsistencyToken;

                return AuthData.GsfId;
            }
        }

        public async Task<UploadDeviceConfigResponse> UploadDeviceConfig(DeviceInfoProvider deviceInfoProvider)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(HeaderProvider.GetDefaultHeaders(AuthData));

            using (MemoryStream memStream = new MemoryStream())
            {
                new UploadDeviceConfigRequest()
                {
                    DeviceConfiguration = deviceInfoProvider.DeviceConfigurationProto
                }.WriteTo(memStream);

                memStream.Position = 0;

                StreamContent streamContent = new StreamContent(memStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");

                HttpResponseMessage response = await HttpClient.PostAsync(Constants.URL_UPLOAD_DEVICE_CONFIG, streamContent);
                UploadDeviceConfigResponse uploadDeviceResponse = ResponseWrapper.Parser.ParseFrom(await response.Content.ReadAsByteArrayAsync()).Payload.UploadDeviceConfigResponse;

                AuthData.DeviceConfigToken = uploadDeviceResponse.UploadDeviceConfigToken;

                return uploadDeviceResponse;
            }
        }

        public async Task<string> GenerateAASToken(string oauthToken)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(HeaderProvider.GetAuthHeaders(AuthData));
            HttpClient.DefaultRequestHeaders.Add("app", "com.android.vending");

            Dictionary<string, string> postData = new Dictionary<string, string>();

            postData.AddRange(ParamProvider.GetDefaultAuthParams(AuthData));
            postData.AddRange(ParamProvider.GetAASTokenParams(oauthToken));

            HttpContent content = new FormUrlEncodedContent(postData);
            HttpResponseMessage response = await HttpClient.PostAsync(Constants.URL_AUTH, content);

            String responseContent = await response.Content.ReadAsStringAsync();

            Dictionary<string, string> responseMap = ParseResponse(responseContent);

            if (responseMap.ContainsKey("Token"))
            {
                return responseMap["Token"];
            }
            else if (responseMap.ContainsKey("Error"))
            {
                throw new Exception(responseMap["Error"]);
            }
            else
            {
                throw new Exception("Authentication failed: Could not generate AAS Token");
            }
        }

        public async Task<string> GenerateToken(string aasToken, Service service)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(HeaderProvider.GetAuthHeaders(AuthData));

            Dictionary<string, string> postData = new Dictionary<string, string>();

            postData.AddRange(ParamProvider.GetDefaultAuthParams(AuthData));
            postData.AddRange(ParamProvider.GetAuthParams(aasToken));

            switch (service)
            {
                case Service.AC2DM:
                    postData["service"] = "ac2dm";
                    postData.Remove("app");
                    break;
                case Service.ANDROID_CHECK_IN_SERVER:
                    postData["oauth2_foreground"] = "0";
                    postData["app"] = "com.google.android.gms";
                    postData["service"] = "AndroidCheckInServer";
                    break;
                case Service.EXPERIMENTAL_CONFIG:
                    postData["service"] = "oauth2:https://www.googleapis.com/auth/experimentsandconfigs";
                    break;
                case Service.NUMBERER:
                    postData["app"] = "com.google.android.gms";
                    postData["service"] = "oauth2:https://www.googleapis.com/auth/numberer";
                    break;
                case Service.GCM:
                    postData["app"] = "com.google.android.gms";
                    postData["service"] = "oauth2:https://www.googleapis.com/auth/gcm";
                    break;
                case Service.GOOGLE_PLAY:
                    //HttpClient.DefaultRequestHeaders.Add("app", "com.google.android.gms");
                    postData["service"] = "oauth2:https://www.googleapis.com/auth/googleplay";
                    break;
                case Service.OAUTHLOGIN:
                    postData["oauth2_foreground"] = "0";
                    postData["app"] = "com.google.android.googlequicksearchbox";
                    postData["service"] = "oauth2:https://www.google.com/accounts/OAuthLogin";
                    postData["callerPkg"] = "com.google.android.googlequicksearchbox";
                    break;
                case Service.ANDROID:
                    postData["service"] = "android";
                    break;
            }

            HttpContent content = new FormUrlEncodedContent(postData);
            HttpResponseMessage response = await HttpClient.PostAsync(Constants.URL_AUTH, content);

            string responseContent = await response.Content.ReadAsStringAsync();

            Dictionary<string, string> responseMap = ParseResponse(responseContent);

            if (responseMap.ContainsKey("Auth"))
            {
                return responseMap["Auth"];
            }
            else if (responseMap.ContainsKey("Error"))
            {
                throw new Exception(responseMap["Error"]);
            }
            else
            {
                throw new Exception("Authentication failed: Could not generate OAuth Token");
            }
        }

        public async Task<TocResponse> toc()
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(HeaderProvider.GetDefaultHeaders(AuthData));

            TocResponse tocResponse = ResponseWrapper.Parser.ParseFrom(await HttpClient.GetStreamAsync(Constants.URL_TOC)).Payload.TocResponse;
            if (!string.IsNullOrEmpty(tocResponse.TosContent) && !string.IsNullOrEmpty(tocResponse.TosToken))
            {
                await acceptTos(tocResponse.TosToken);
            }
            if (!string.IsNullOrEmpty(tocResponse.Cookie))
            {
                AuthData.DfeCookie = tocResponse.Cookie;
            }

            return tocResponse;
        }

        private async Task<AcceptTosResponse> acceptTos(string tosToken)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(HeaderProvider.GetDefaultHeaders(AuthData));

            Dictionary<string, string> postData = new Dictionary<string, string>();

            postData["tost"] = tosToken;
            postData["toscme"] = "false";

            HttpContent content = new FormUrlEncodedContent(postData);
            HttpResponseMessage response = await HttpClient.PostAsync(Constants.URL_TOS_ACCEPT, content);

            return ResponseWrapper.Parser.ParseFrom(response.Content.ReadAsStream()).Payload.AcceptTosResponse;
        }

        private Dictionary<string, string> ParseResponse(string response)
        {
             return response.Split("\n")
                .Select(value => value.Split('='))
                .ToDictionary(pair => pair[0], pair => String.Join("=", pair.SubArray(1, pair.Length - 1)));
        }
    }
}

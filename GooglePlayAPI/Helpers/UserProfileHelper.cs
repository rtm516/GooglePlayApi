using GooglePlayApi.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using GooglePlayApi.Providers;
using GooglePlayApi.Proto;

namespace GooglePlayApi.Helpers
{
    public class UserProfileHelper
    {
        public AuthData AuthData { get; private set; }
        public HttpClient HttpClient { get; private set; }

        public UserProfileHelper(AuthData authData, HttpClient httpClient)
        {
            AuthData = authData;
            HttpClient = httpClient;
        }
        public async Task<UserProfileResponse> GetUserProfileResponse()
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(HeaderProvider.GetDefaultHeaders(AuthData));

            HttpResponseMessage response = await HttpClient.GetAsync(Constants.URL_USER_PROFILE);

            if (response.IsSuccessStatusCode)
            {
                return ResponseWrapperApi.Parser.ParseFrom(await response.Content.ReadAsByteArrayAsync()).Payload.UserProfileResponse;
            }
            else
            {
                throw new Exception("Failed to fetch user profile: " + response.StatusCode);
            }

        }

        public async Task<Models.UserProfile> getUserProfile()
        {
            return new Models.UserProfile((await GetUserProfileResponse()).UserProfile);
        }

    }
}

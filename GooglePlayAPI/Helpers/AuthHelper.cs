using GooglePlayApi.Models;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace GooglePlayApi.Helpers
{
    public class AuthHelper
    {
        private static HttpClient HttpClient = new HttpClient();

        public static async Task<AuthData> Build(string email, string aasToken, string propertiesFile)
        {
            DeviceInfoProvider deviceInfoProvider = new DeviceInfoProvider(propertiesFile, CultureInfo.CurrentCulture.Name);

            AuthData authData = new AuthData(email, aasToken);
            authData.DeviceInfoProvider = deviceInfoProvider;
            authData.Locale = CultureInfo.CurrentCulture;

            GooglePlayApi api = new GooglePlayApi(authData, HttpClient);

            // Check if we really have an aas token
            if (aasToken.StartsWith("oauth2_4/"))
            {
                aasToken = await api.GenerateAASToken(aasToken);
                authData.AasToken = aasToken;
            }

            string gsfId = await api.GenerateGsfId(deviceInfoProvider);
            authData.GsfId = gsfId;

            Proto.UploadDeviceConfigResponse deviceConfigResponse = await api.UploadDeviceConfig(deviceInfoProvider);
            authData.DeviceConfigToken = deviceConfigResponse.UploadDeviceConfigToken;

            string ac2dm = await api.GenerateToken(aasToken, Service.AC2DM);
            authData.Ac2dmToken = ac2dm;

            string gcmToken = await api.GenerateToken(aasToken, Service.GCM);
            authData.GcmToken = gcmToken;

            string token = await api.GenerateToken(aasToken, Service.GOOGLE_PLAY);
            authData.AuthToken = token;

            await api.toc();

            authData.UserProfile = await new UserProfileHelper(authData, HttpClient).getUserProfile();

            return authData;
        }
    }
}

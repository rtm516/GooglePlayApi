using GooglePlayApi.Models;
using System.Collections.Generic;
using System.Globalization;

namespace GooglePlayApi.Providers
{
    public class ParamProvider
    {
        public static Dictionary<string, string> GetDefaultAuthParams(AuthData authData)
        {
            Dictionary<string, string> postData = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(authData.GsfId))
            {
                postData["androidId"] = authData.GsfId;
            }

            postData["sdk_version"] = authData.DeviceInfoProvider?.SdkVersion.ToString();
            postData["Email"] = authData.Email;
            postData["google_play_services_version"] = authData.DeviceInfoProvider?.PlayServicesVersion.ToString();
            postData["device_country"] = new RegionInfo(authData.Locale.LCID).TwoLetterISORegionName.ToLower();
            postData["lang"] = authData.Locale.Name.ToLower();
            postData["callerSig"] = "38918a453d07199354f8b19af05ec6562ced5788";

            return postData;
        }

        public static Dictionary<string, string> GetAuthParams(string aasToken)
        {
            Dictionary<string, string> postData = new Dictionary<string, string>();

            postData["app"] = "com.android.vending";
            postData["client_sig"] = "38918a453d07199354f8b19af05ec6562ced5788";
            postData["callerPkg"] = "com.google.android.gms";
            postData["Token"] = aasToken;
            postData["oauth2_foreground"] = "1";
            postData["token_request_options"] = "CAA4AVAB";
            postData["check_email"] = "1";
            postData["system_partition"] = "1";

            return postData;
        }

        public static Dictionary<string, string> GetAASTokenParams(string oauthToken)
        {
            Dictionary<string, string> postData = new Dictionary<string, string>();

            postData["service"] = "ac2dm";
            postData["add_account"] = "1";
            postData["get_accountid"] = "1";
            postData["ACCESS_TOKEN"] = "1";
            postData["callerPkg"] = "com.google.android.gms";
            postData["Token"] = oauthToken;

            return postData;
        }
    }
}

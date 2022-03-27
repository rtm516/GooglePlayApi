using System.Globalization;

namespace GooglePlayApi.Models
{
    public class AuthData
    {
        public AuthData(string email, string aasToken)
        {
            Email = email;
            AasToken = aasToken;
        }

        public string Email { get; set; }
        public string AasToken { get; set; }
        public bool IsAnonymous { get; set; }
        public string AuthToken { get; set; }
        public string GsfId { get; set; }
        public string Ac2dmToken { get; set; }
        public string DeviceCheckInConsistencyToken { get; set; }
        public string DeviceConfigToken { get; set; }
        public string GcmToken { get; set; }
        public string DfeCookie { get; set; }
        public CultureInfo Locale { get; set; } = CultureInfo.CurrentCulture;
        public DeviceInfoProvider DeviceInfoProvider { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
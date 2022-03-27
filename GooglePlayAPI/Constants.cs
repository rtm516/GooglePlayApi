namespace GooglePlayApi
{
    internal class Constants
    {
        private const string URL_BASE = "https://android.clients.google.com/";
        private const string URL_FDFE = URL_BASE + "fdfe/";
        public const string URL_CHECK_IN = URL_BASE + "checkin";
        public const string URL_AUTH = URL_BASE + "auth";
        public const string URL_UPLOAD_DEVICE_CONFIG = URL_FDFE + "uploadDeviceConfig";
        public const string URL_DETAILS = URL_FDFE + "details";
        public const string DELIVERY_URL = URL_FDFE + "delivery";
        public const string PURCHASE_URL = URL_FDFE + "purchase";
        public const string URL_TOC = URL_FDFE + "toc";
        public const string URL_TOS_ACCEPT = URL_FDFE + "acceptTos";
        public const string URL_USER_PROFILE = URL_FDFE + "api/userProfile";

        public const int BUILD_VERSION_SDK = 28;
        public const int PLAY_SERVICES_VERSION_CODE = 19629032;
    }
}

using GooglePlayApi.Proto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace GooglePlayApi
{
    public class DeviceInfoProvider
    {
        public Dictionary<string, string> Properties { get; }
        public string LocaleString { get; }

        public int SdkVersion => int.Parse(Properties["Build.VERSION.SDK_INT"]);
        public int PlayServicesVersion => int.Parse(Properties["GSF.version"]);
        public string MccMnc => Properties["SimOperator"];
        public string AuthUserAgentString => $"GoogleAuth/1.4 ({Properties["Build.DEVICE"]} {Properties["Build.ID"]})";
        [JsonIgnore]
        public string UserAgentString
        {
            get
            {
                List<string> args = new List<string>();
                args.Add("api=3");
                args.Add("versionCode=" + Properties["Vending.version"]);
                args.Add("sdk=" + Properties["Build.VERSION.SDK_INT"]);
                args.Add("device=" + Properties["Build.DEVICE"]);
                args.Add("hardware=" + Properties["Build.HARDWARE"]);
                args.Add("product=" + Properties["Build.PRODUCT"]);
                args.Add("platformVersionRelease=" + Properties["Build.VERSION.RELEASE"]);
                args.Add("model=" + Properties["Build.MODEL"]);
                args.Add("buildId=" + Properties["Build.ID"]);
                args.Add("isWideScreen=0");
                args.Add("supportedAbis=" + Properties["Platforms"].Replace(',', ';'));

                return $"Android-Finsky/{Properties["Vending.versionString"].Split(' ')[0]} ({string.Join(",", args)})";
            }
        }

        [JsonConstructor]
        public DeviceInfoProvider() { }

        public DeviceInfoProvider(string propertiesFile, string localeString)
        {
            LocaleString = localeString;

            Properties = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(propertiesFile))
            {
                string key = row.Split('=')[0];
                if (key.StartsWith("#")) continue;
                Properties.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
            }
        }

        public AndroidCheckinRequest GenerateAndroidCheckInRequest()
        {
            return new AndroidCheckinRequest()
            {
                Id = 0,
                Checkin = new AndroidCheckinProto()
                {
                    Build = new AndroidBuildProto()
                    {
                        Id = Properties["Build.FINGERPRINT"],
                        Product = Properties["Build.HARDWARE"],
                        Carrier = Properties["Build.BRAND"],
                        Radio = Properties["Build.RADIO"],
                        Bootloader = Properties["Build.BOOTLOADER"],
                        Device = Properties["Build.DEVICE"],
                        SdkVersion = int.Parse(Properties["Build.VERSION.SDK_INT"]),
                        Model = Properties["Build.MODEL"],
                        Manufacturer = Properties["Build.MANUFACTURER"],
                        BuildProduct = Properties["Build.PRODUCT"],
                        Client = Properties["Client"],
                        OtaInstalled = bool.Parse(Properties.GetValueOrDefault("OtaInstalled", "false")),
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000,
                        GoogleServices = int.Parse(Properties["GSF.version"]),
                    },
                    LastCheckinMsec = 0,
                    CellOperator = Properties["CellOperator"],
                    SimOperator = Properties["SimOperator"],
                    Roaming = Properties["Roaming"],
                    UserNumber = 0
                },
                Locale = LocaleString,
                TimeZone = Properties["TimeZone"],
                Version = 3,
                DeviceConfiguration = DeviceConfigurationProto,
                Fragment = 0
            };
        }

        [JsonIgnore]
        public DeviceConfigurationProto DeviceConfigurationProto
        {
            get
            {
                DeviceConfigurationProto deviceConfiguration = new DeviceConfigurationProto()
                {
                    TouchScreen = int.Parse(Properties["TouchScreen"]),
                    Keyboard = int.Parse(Properties["Keyboard"]),
                    Navigation = int.Parse(Properties["Navigation"]),
                    ScreenLayout = int.Parse(Properties["ScreenLayout"]),
                    HasHardKeyboard = bool.Parse(Properties["HasHardKeyboard"]),
                    HasFiveWayNavigation = bool.Parse(Properties["HasFiveWayNavigation"]),
                    LowRamDevice = int.Parse(Properties.GetValueOrDefault("LowRamDevice", "0")),
                    MaxNumOfCPUCores = int.Parse(Properties.GetValueOrDefault("MaxNumOfCPUCores", "8")),
                    TotalMemoryBytes = long.Parse(Properties.GetValueOrDefault("TotalMemoryBytes", "8589935000")),
                    DeviceClass = 0,
                    ScreenDensity = int.Parse(Properties["Screen.Density"]),
                    ScreenWidth = int.Parse(Properties["Screen.Width"]),
                    ScreenHeight = int.Parse(Properties["Screen.Height"]),
                    GlEsVersion = int.Parse(Properties["GL.Version"])
                };
                deviceConfiguration.NativePlatform.AddRange(Properties["Platforms"].Split(",").ToList());
                deviceConfiguration.SystemSharedLibrary.AddRange(Properties["SharedLibraries"].Split(",").ToList());
                deviceConfiguration.SystemAvailableFeature.AddRange(Properties["Features"].Split(",").ToList());
                deviceConfiguration.SystemSupportedLocale.AddRange(Properties["Locales"].Split(",").ToList());
                deviceConfiguration.GlExtension.AddRange(Properties["GL.Extensions"].Split(",").ToList());
                deviceConfiguration.DeviceFeature.AddRange(Properties["Features"].Split(",").ToList().Select(feature => new DeviceFeature() { Name = feature, Value = 0 }).ToList());

                return deviceConfiguration;
            }
        }
    }
}
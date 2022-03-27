using GooglePlayApi.Models;
using System.Collections.Generic;

namespace GooglePlayApi.Providers
{
    public class HeaderProvider
    {
        public static Dictionary<string, string> GetDefaultHeaders(AuthData authData)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + authData.AuthToken);
            headers.Add("User-Agent", authData.DeviceInfoProvider?.UserAgentString);
            headers.Add("X-DFE-Device-Id", authData.GsfId);
            headers.Add("Accept-Language", authData.Locale.Name.Replace("_", "-"));
            headers.Add("X-DFE-Encoded-Targets", "CAESN/qigQYC2AMBFfUbyA7SM5Ij/CvfBoIDgxHqGP8R3xzIBvoQtBKFDZ4HAY4FrwSVMasHBO0O2Q8akgYRAQECAQO7AQEpKZ0CnwECAwRrAQYBr9PPAoK7sQMBAQMCBAkIDAgBAwEDBAICBAUZEgMEBAMLAQEBBQEBAcYBARYED+cBfS8CHQEKkAEMMxcBIQoUDwYHIjd3DQ4MFk0JWGYZEREYAQOLAYEBFDMIEYMBAgICAgICOxkCD18LGQKEAcgDBIQBAgGLARkYCy8oBTJlBCUocxQn0QUBDkkGxgNZQq0BZSbeAmIDgAEBOgGtAaMCDAOQAZ4BBIEBKUtQUYYBQscDDxPSARA1oAEHAWmnAsMB2wFyywGLAxol+wImlwOOA80CtwN26A0WjwJVbQEJPAH+BRDeAfkHK/ABASEBCSAaHQemAzkaRiu2Ad8BdXeiAwEBGBUBBN4LEIABK4gB2AFLfwECAdoENq0CkQGMBsIBiQEtiwGgA1zyAUQ4uwS8AwhsvgPyAcEDF27vApsBHaICGhl3GSKxAR8MC6cBAgItmQYG9QIeywLvAeYBDArLAh8HASI4ELICDVmVBgsY/gHWARtcAsMBpALiAdsBA7QBpAJmIArpByn0AyAKBwHTARIHAX8D+AMBcRIBBbEDmwUBMacCHAciNp0BAQF0OgQLJDuSAh54kwFSP0eeAQQ4M5EBQgMEmwFXywFo0gFyWwMcapQBBugBPUW2AVgBKmy3AR6PAbMBGQxrUJECvQR+8gFoWDsYgQNwRSczBRXQAgtRswEW0ALMAREYAUEBIG6yATYCRE8OxgER8gMBvQEDRkwLc8MBTwHZAUOnAXiiBakDIbYBNNcCIUmuArIBSakBrgFHKs0EgwV/G3AD0wE6LgECtQJ4xQFwFbUCjQPkBS6vAQqEAUZF3QIM9wEhCoYCQhXsBCyZArQDugIziALWAdIBlQHwBdUErQE6qQaSA4EEIvYBHir9AQVLmgMCApsCKAwHuwgrENsBAjNYswEVmgIt7QJnN4wDEnta+wGfAcUBxgEtEFXQAQWdAUAeBcwBAQM7rAEJATJ0LENrdh73A6UBhAE+qwEeASxLZUMhDREuH0CGARbd7K0GlQo");
            headers.Add("X-DFE-Phenotype", "H4sIAAAAAAAAAB3OO3KjMAAA0KRNuWXukBkBQkAJ2MhgAZb5u2GCwQZbCH_EJ77QHmgvtDtbv-Z9_H63zXXU0NVPB1odlyGy7751Q3CitlPDvFd8lxhz3tpNmz7P92CFw73zdHU2Ie0Ad2kmR8lxhiErTFLt3RPGfJQHSDy7Clw10bg8kqf2owLokN4SecJTLoSwBnzQSd652_MOf2d1vKBNVedzg4ciPoLz2mQ8efGAgYeLou-l-PXn_7Sna1MfhHuySxt-4esulEDp8Sbq54CPPKjpANW-lkU2IZ0F92LBI-ukCKSptqeq1eXU96LD9nZfhKHdtjSWwJqUm_2r6pMHOxk01saVanmNopjX3YxQafC4iC6T55aRbC8nTI98AF_kItIQAJb5EQxnKTO7TZDWnr01HVPxelb9A2OWX6poidMWl16K54kcu_jhXw-JSBQkVcD_fPsLSZu6joIBAAA");
            headers.Add("X-DFE-Client-Id", "am-android-google");
            headers.Add("X-DFE-Network-Type", "4");
            headers.Add("X-DFE-Content-Filters", "");
            headers.Add("X-Limit-Ad-Tracking-Enabled", "false");
            headers.Add("X-Ad-Id", "LawadaMera");
            headers.Add("X-DFE-UserLanguages", authData.Locale.Name);
            headers.Add("X-DFE-Request-Params", "timeoutMs=4000");

            if (!string.IsNullOrEmpty(authData.DeviceCheckInConsistencyToken))
            {
                headers.Add("X-DFE-Device-Checkin-Consistency-Token", authData.DeviceCheckInConsistencyToken);
            }

            if (!string.IsNullOrEmpty(authData.DeviceConfigToken))
            {
                headers.Add("X-DFE-Device-Config-Token", authData.DeviceConfigToken);
            }

            if (!string.IsNullOrEmpty(authData.DfeCookie))
            {
                headers.Add("X-DFE-Cookie", authData.DfeCookie);
            }

            string mccMnc = authData.DeviceInfoProvider?.MccMnc;
            if (!string.IsNullOrEmpty(mccMnc))
            {
                headers.Add("X-DFE-MCCMNC", mccMnc);
            }

            return headers;
        }

        public static Dictionary<string, string> GetAuthHeaders(AuthData authData)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "app", "com.google.android.gms" },
                { "User-Agent", authData.DeviceInfoProvider.AuthUserAgentString }
            };

            if (!string.IsNullOrEmpty(authData.GsfId))
            {
                headers.Add("device", authData.GsfId);
            }

            return headers;
        }
    }
}

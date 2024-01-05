using GooglePlayApi;
using GooglePlayApi.Helpers;
using GooglePlayApi.Models;
using GooglePlayApi.Popup;
using GooglePlayApi.Proto;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bootstrap
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            string cacheFile = "AuthData.json";
            string deviceProperties = "violet.properties";
            JsonSerializerOptions serializeOptions = new JsonSerializerOptions { Converters = { new CultureInfoJsonConverter() } };

            AuthData authData;

            if (!File.Exists(cacheFile))
            {
                (string Email, string OAuthToken) authResponse = AuthPopupForm.GetOAuthToken();
                authData = await AuthHelper.Build(authResponse.Email, authResponse.OAuthToken, deviceProperties);
            }
            else
            {
                // Get cached auth
                using (FileStream readStream = File.OpenRead(cacheFile))
                    authData = await JsonSerializer.DeserializeAsync<AuthData>(readStream, serializeOptions);

                // Re-aquire tokens
                try
                {
                    authData = await AuthHelper.Build(authData.Email, authData.AasToken, deviceProperties);
                }
                catch (Exception ex)
                {
                    // Auth failed so request new auth as its likely expired
                    (string Email, string OAuthToken) authResponse = AuthPopupForm.GetOAuthToken();
                    authData = await AuthHelper.Build(authResponse.Email, authResponse.OAuthToken, deviceProperties);
                }
            }

            // Save latest auth to cache file
            try
            {
                using (FileStream writeStream = File.Create(cacheFile))
                    await JsonSerializer.SerializeAsync(writeStream, authData, serializeOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
            }

            AppDetailsHelper appDetailsHelper = new AppDetailsHelper(authData, new HttpClient());
            PurchaseHelper purchaseHelper = new PurchaseHelper(authData, new HttpClient());

            // Get package info
            DetailsResponse appDetails = await appDetailsHelper.GetAppByPackageName("com.mojang.minecraftedu");
            Console.WriteLine($"{appDetails.Item.Title} by {appDetails.Item.Creator} - {appDetails.Item.Details.AppDetails.VersionString} ({appDetails.Item.Details.AppDetails.VersionCode})");

            // 'Buy' the free app if needed
            //BuyResponse appBuy = await purchaseHelper.GetBuyResponse(appDetails.Item.Details.AppDetails.PackageName, appDetails.Item.Details.AppDetails.VersionCode, appDetails.Item.Offer[0].OfferType);
            //Console.WriteLine(appBuy);

            // Get the new app details
            //appDetails = await appDetailsHelper.GetAppByPackageName("com.mojang.minecraftedu");

            // Check if there is a beta/testing program
            if (appDetails.Item.Details.AppDetails.TestingProgramInfo != null)
            {
                // If we are not in it join
                Console.WriteLine($"In beta: {appDetails.Item.Details.AppDetails.TestingProgramInfo.Subscribed}");
                if (!appDetails.Item.Details.AppDetails.TestingProgramInfo.Subscribed)
                {
                    Console.WriteLine("Subscribing to beta");
                    TestingProgramResponse testingProgram = await appDetailsHelper.TestingProgram(appDetails.Item.Details.AppDetails.PackageName, false);
                    Console.WriteLine(testingProgram.Result.Details);
                }
            }

            // Get the delivery details for download
            DeliveryResponse appDelivery = await purchaseHelper.GetDeliveryResponse(appDetails.Item.Details.AppDetails.PackageName, appDetails.Item.Details.AppDetails.VersionCode, appDetails.Item.Offer[0].OfferType);
            //Console.WriteLine(appDelivery);

            string apkFolder = "./" + appDetails.Item.Details.AppDetails.PackageName + "/";

            if (!Directory.Exists(apkFolder))
            {
                Directory.CreateDirectory(apkFolder);
            }

            // Download all apk parts
            Console.WriteLine("Downloading all apk parts...");
            WebClient webClient = new WebClient();
            await webClient.DownloadFileTaskAsync(appDelivery.AppDeliveryData.DownloadUrl, apkFolder + "base.apk");
            foreach (SplitDeliveryData splitDeliveryData in appDelivery.AppDeliveryData.SplitDeliveryData)
            {
                await webClient.DownloadFileTaskAsync(new Uri(splitDeliveryData.DownloadUrl), apkFolder + "split_" + splitDeliveryData.Name + ".apk");
            }
            Console.WriteLine("Downloaded all apk parts!");
        }
    }
}

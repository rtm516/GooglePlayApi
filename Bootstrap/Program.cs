﻿using GooglePlayApi;
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
            string deviceProperties = "octopus.properties";
            JsonSerializerOptions serializeOptions = new JsonSerializerOptions { Converters = { new CultureInfoJsonConverter() } };

            AuthData authData;

            if (!File.Exists(cacheFile))
            {
                // cookie `oauth_token` from https://accounts.google.com/EmbeddedSetup/identifier?flowName=EmbeddedSetupAndroid
                authData = await AuthHelper.Build("rtm615@gmail.com", AuthPopupForm.GetOauthToken(), deviceProperties);
            }
            else
            {
                // Get cached auth
                using (FileStream readStream = File.OpenRead(cacheFile))
                    authData = await JsonSerializer.DeserializeAsync<AuthData>(readStream, serializeOptions);

                // Re-aquire tokens
                // TODO: Check if they are still valid
                authData = await AuthHelper.Build(authData.Email, authData.AasToken, deviceProperties);
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


            DetailsResponse appDetails = await new AppDetailsHelper(authData, new HttpClient()).GetAppByPackageName("com.mojang.minecraftedu");
            Console.WriteLine($"{appDetails.Item.Title} by {appDetails.Item.Creator} - {appDetails.Item.Details.AppDetails.VersionString} ({appDetails.Item.Details.AppDetails.VersionCode})");

            PurchaseHelper purchaseHelper = new PurchaseHelper(authData, new HttpClient());

            BuyResponse appBuy = await purchaseHelper.GetBuyResponse(appDetails.Item.Details.AppDetails.PackageName, appDetails.Item.Details.AppDetails.VersionCode, appDetails.Item.Offer[0].OfferType);
            //Console.WriteLine(appBuy);

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

using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GooglePlayApi.Popup
{
    public partial class AuthPopupForm : Form
    {
        private static string OAuthToken { get; set; } = "";
        private static string Email { get; set; } = "";

        public AuthPopupForm()
        {
            CefSettings cefSettings = new CefSettings();
            cefSettings.UserAgent = ProductName + "/" + ProductVersion; // The default user agent stops google from allowing auth
            Cef.Initialize(cefSettings);

            InitializeComponent();

            chromiumWebBrowser1.LoadUrl("https://accounts.google.com/EmbeddedSetup/identifier?flowName=EmbeddedSetupAndroid");
        }

        private void chromiumWebBrowser1_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading) return;

            var visitor = new CookieMonster(async all_cookies =>
            {
                var sb = new StringBuilder();
                foreach (var nameValue in all_cookies)
                {
                    if (nameValue.Item1 == "oauth_token")
                    {
                        this.DialogResult = DialogResult.OK;
                        OAuthToken = nameValue.Item2;
                        JavascriptResponse response = await chromiumWebBrowser1.EvaluateScriptAsync("document.getElementById('profileIdentifier').innerHTML");
                        Email = (string)response.Result;
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            this.Close();
                        }));

                    }
                }
            });
            Cef.GetGlobalCookieManager().VisitAllCookies(visitor);
        }

        public static (string Email, string OauthToken) GetOAuthToken()
        {
            AuthPopupForm popupForm = new AuthPopupForm();
            popupForm.ShowDialog();

            return (Email: Email, OAuthToken: OAuthToken);
        }
    }
}

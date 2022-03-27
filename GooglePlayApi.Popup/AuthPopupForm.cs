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
        private static string OauthToken { get; set; }

        public AuthPopupForm()
        {
            CefSettings cefSettings = new CefSettings();
            cefSettings.UserAgent = ProductName + "/" + ProductVersion; // The default user agent stops google from allowing auth
            Cef.Initialize(cefSettings);

            InitializeComponent();

            this.chromiumWebBrowser1.LoadUrl("https://accounts.google.com/EmbeddedSetup/identifier?flowName=EmbeddedSetupAndroid");
        }

        private void chromiumWebBrowser1_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading) return;

            var visitor = new CookieMonster(all_cookies =>
            {
                var sb = new StringBuilder();
                foreach (var nameValue in all_cookies)
                {
                    if (nameValue.Item1 == "oauth_token")
                    {
                        this.DialogResult = DialogResult.OK;
                        OauthToken = nameValue.Item2;
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            this.Close();
                        }));

                    }
                }
            });
            Cef.GetGlobalCookieManager().VisitAllCookies(visitor);
        }

        public static string GetOauthToken()
        {
            AuthPopupForm popupForm = new AuthPopupForm();
            if (popupForm.ShowDialog() == DialogResult.OK)
            {
                return OauthToken;
            }

            return "";
        }
    }
}

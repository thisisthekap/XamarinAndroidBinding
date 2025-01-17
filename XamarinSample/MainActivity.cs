using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Com.Appsflyer;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;

namespace XamarinSample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
                   AutoVerify = true,
                   Categories = new[]
                   {
                        Android.Content.Intent.CategoryDefault,
                        Android.Content.Intent.CategoryBrowsable
                   },
                   DataScheme = "sdktest")]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
                   Categories = new[]
                   {
                        Android.Content.Intent.CategoryDefault,
                        Android.Content.Intent.CategoryBrowsable
                   },
                   DataScheme = "https",
                   DataHosts = new[]
                   {
                       "sdk-test.onelink.me",
                       "click.af-sup.com"
                   }
                   )]
    public class MainActivity : AppCompatActivity
    {
        public AppCompatTextView gcdTextView;
        public AppCompatTextView udlTextView;
        public FloatingActionButton purchaseButton;
        FloatingActionButton fab;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
            purchaseButton = FindViewById<FloatingActionButton>(Resource.Id.purchase_button);
            purchaseButton.Click += PurchaseButtonClick;
            gcdTextView = FindViewById<AppCompatTextView>(Resource.Id.gcd_text_view);
            udlTextView = FindViewById<AppCompatTextView>(Resource.Id.udl_text_view);
            ConversionListener cl = new ConversionListener(this);
            DeepLinkListener dl = new DeepLinkListener(this);
            AppsFlyerLib.Instance.AddPushNotificationDeepLinkPath(new string[] { "key1", "key2" });
            AppsFlyerLib.Instance.SetDebugLog(true);
            //AppsFlyerLib.Instance.SetLogLevel(AFLogger.LogLevel.Verbose); // Enable verbose logs for debugging
            AppsFlyerLib.Instance.Init("4UGrDF4vFvPLbHq5bXtCza", cl, Application);
            AppsFlyerLib.Instance.SetAppInviteOneLink("E2bM"); // Replace with OneLink ID from your AppsFlyer account
            AppsFlyerLib.Instance.SetSharingFilter(new string[]{"test", "partner_int"});
            Dictionary<string, Java.Lang.Object> partnerData =
                new Dictionary<string, Java.Lang.Object>();
            partnerData.Add("id", "test_id");
            partnerData.Add("value", "test_value");
            AppsFlyerLib.Instance.SetPartnerData("test_partner", partnerData);
            AppsFlyerLib.Instance.RegisterConversionListener(this, cl);
            AppsFlyerLib.Instance.SubscribeForDeepLink(dl);
            AppsFlyerLib.Instance.SetDisableAdvertisingIdentifiers(false);
            AppsFlyerLib.Instance.Start(this, "4UGrDF4vFvPLbHq5bXtCza"); // Replace with your app DevKey
        }

        protected override void OnStop()
        {
            base.OnStop();
            gcdTextView.Text = "Conversion Data";
            udlTextView.Text = "onDeepLinking";
        }

        protected override void OnResume()
        {
            base.OnResume();
            Console.WriteLine(gcdTextView.Text);
            Console.WriteLine(udlTextView.Text);
        }

        private void PurchaseButtonClick(object sender, EventArgs eventArgs)
        {
            Dictionary<string, Java.Lang.Object> eventValues = new Dictionary<string, Java.Lang.Object>();
            eventValues.Add(AFInAppEventParameterName.Price, 2);
            eventValues.Add(AFInAppEventParameterName.Currency, "USD");
            eventValues.Add(AFInAppEventParameterName.Quantity, "1");
            AppsFlyerLib.Instance.LogEvent(this.BaseContext, AFInAppEventType.Purchase, eventValues);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;

            // Correct User Invite implementation
            Com.Appsflyer.Share.LinkGenerator linkGenerator = Com.Appsflyer.Share.ShareInviteHelper.GenerateInviteUrl(view.Context);
            linkGenerator.SetCampaign("my_campaign");
            linkGenerator.AddParameter("af_cost_value", "2.5");
            linkGenerator.AddParameter("af_cost_currency", "USD");
            OneLinkResponseListener listener = new OneLinkResponseListener(view);
            linkGenerator.GenerateLink(view.Context, listener);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }


    class OneLinkResponseListener : Java.Lang.Object, CreateOneLinkHttpTask.IResponseListener
    { 
        View view;

        public OneLinkResponseListener(View view)
        {
            this.view = view;
        }

        public void OnResponse(string p0)
        {
            string message = "Link generated sucessfully: " + p0;
            Snackbar.Make(view, message, Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
            Console.WriteLine(message);
        }

        public void OnResponseError(string p0)
        {
            string message = "Link was NOT generated. Error: " + p0;
            Snackbar.Make(view, message, Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
            Console.WriteLine(message);
        }
    }
}
using System;
using System.IO;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using GhostlyLog;
using GhostlyLog.ViewModel;
using OpenFeasyo.Platform.Data;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace GhostlyGame
{
    [Activity(Label = "Ghostly Log",
        MainLauncher = true,
        Icon = "@mipmap/ic_ghostlylog",
        Theme = "@style/BrowserTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class GhostlyLogActivity : AppCompatActivity
    {
        public static string FolderPath { get; private set; }

        public static GhostlyLogActivity Instance;

        protected override void OnCreate(Bundle bundle)
        {
            GhostlyLog.BrowserPage.HomeLocation = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            SeriousGames.HomeLocation = GhostlyLog.BrowserPage.HomeLocation;
            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            Instance = this;

            this.RequestPermissions(new String[] {
                        Manifest.Permission.ReadExternalStorage,
            }, 1);

            SetContentView(Resource.Layout.GhostlyLogMain);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = SeriousGames.GetPatientDirectory(SeriousGames.CurrentPatient);

            FolderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData));
            BrowserPage mainPage = new BrowserPage(VersionTracking.CurrentVersion);
            mainPage.DetailedPageRequested += MainPage_DetailedPageRequested;
            Android.Support.V4.App.Fragment mainFragment = mainPage.CreateSupportFragment(this);
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.fragment_frame_layout, mainFragment)
                .Commit();

            SupportFragmentManager.BackStackChanged += (sender, e) =>
            {
                bool hasBack = SupportFragmentManager.BackStackEntryCount > 0;
                SupportActionBar.SetHomeButtonEnabled(hasBack);
                SupportActionBar.SetDisplayHomeAsUpEnabled(hasBack);
                SupportActionBar.Title = hasBack ? "Detailed report" : "Ghostly Log";
            };
        }

        private void MainPage_DetailedPageRequested(ContentPage page)
        {
            Android.Support.V4.App.Fragment detailsPage = page.CreateSupportFragment(this);
            SupportFragmentManager
                .BeginTransaction()
                .AddToBackStack(null)
                .Replace(Resource.Id.fragment_frame_layout, detailsPage)
                .Commit();
        }

        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            if (item.ItemId == global::Android.Resource.Id.Home && SupportFragmentManager.BackStackEntryCount > 0)
            {
                SupportFragmentManager.PopBackStack();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        //public void NavigateToNoteEntryPage(C3dFile file)
        //{
        //    Android.Support.V4.App.Fragment noteEntryPage = new DetailedReportPage
        //    {
        //        BindingContext = file
        //    }.CreateSupportFragment(this);
        //    SupportFragmentManager
        //        .BeginTransaction()
        //        .AddToBackStack(null)
        //        .Replace(Resource.Id.fragment_frame_layout, noteEntryPage)
        //        .Commit();
        //}

        public void NavigateBack()
        {
            SupportFragmentManager.PopBackStack();
        }
    }
}
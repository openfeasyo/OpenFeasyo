using Android;
using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Views;
using OpenFeasyo.Platform.Controls.Drivers;
using OpenFeasyo.Platform.Controls.Analysis;
using OpenFeasyo.Platform.Data;
using OpenFeasyo.Platform.Platform;
using System.Collections.Generic;
using System;
using Android.Runtime;

namespace LoopLib
{
    [Activity(
        Label = "@string/app_name", 
        Theme = "@style/AppTheme",
        ScreenOrientation = ScreenOrientation.SensorLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden,
        MainLauncher = true)]
    public class MainActivity : Microsoft.Xna.Framework.AndroidGameActivity
    {
        public static MainActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);


            base.OnCreate(savedInstanceState);
            EnableImmersiveMode();

            Instance = this;



            InputDeviceManager.Instance = new StaticDriverManager();
            InputAnalyzerManager.Instance = new StaticAnalysisManager();
            SeriousGames.CurrentGame = new ConfiguredGame();
            SeriousGames.CurrentGame.Name = "Loop";
            SeriousGames.HomeLocation = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            UIThread.Instance = new AndroidUIThread(this);


            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                // TODO check whether we need to show permission rationalle
                this.RequestPermissions(new string[] {
                            Manifest.Permission.WakeLock,
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage
                }, 1);
            }

            var g = new LoopGame();
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }

        public void EnableImmersiveMode()
        {
            int uiOptions = (int)this.Window.DecorView.SystemUiVisibility;
            uiOptions |= (int)SystemUiFlags.LowProfile;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;
            this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }

        private Dictionary<int, Action<bool>> permissionRequests = new Dictionary<int, Action<bool>>();

        public void RequestPermissions(string[] permissions, int requestCode, Action<bool> action)
        {
            // On older devices, permisisons are always granted while the app installation
            if (Android.OS.Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                action.Invoke(true);
                return;
            }

            List<string> neededPermissions = new List<string>();
            foreach (string permission in permissions)
            {
                if (CheckSelfPermission(permission) != Permission.Granted)
                {
                    neededPermissions.Add(permission);
                }
            }
            if (neededPermissions.Count > 0)
            {
                RequestPermissions(neededPermissions.ToArray(), requestCode);
                permissionRequests.Add(requestCode, action);
            }
            else
            {
                action.Invoke(true);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (permissionRequests.ContainsKey(requestCode))
            {
                bool granted = true;
                foreach (Permission p in grantResults)
                {
                    granted = granted && p == Permission.Granted;
                }

                Action<bool> action = permissionRequests[requestCode];
                permissionRequests.Remove(requestCode);
                action.Invoke(granted);
            }
        }
    }
}
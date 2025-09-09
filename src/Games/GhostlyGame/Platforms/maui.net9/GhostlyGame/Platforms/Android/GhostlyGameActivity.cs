using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Content.PM;
using Android.Views;


namespace GhostlyLib;


[Activity(Theme = "@style/Maui.SplashTheme",
    ScreenOrientation = ScreenOrientation.SensorLandscape,
    MainLauncher = false,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class GhostlyGameActivity : AndroidGameActivity
{
    public static GhostlyGameActivity Instance { get; private set; }

    private GhostlyLib.GhostlyGame _game;
    private Android.Views.View? _view;

    protected override void OnCreate(Bundle bundle)
    {
        this.Window.AddFlags(WindowManagerFlags.Fullscreen);
        this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
        base.OnCreate(bundle);

        EnableImmersiveMode();

        _game = new GhostlyLib.GhostlyGame();
        _view = _game.Services.GetService(typeof(Android.Views.View)) as Android.Views.View;

        SetContentView(_view);
        _game.Run();
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



    protected override void OnStop()
    {
        base.OnStop();
        this.Finish();
        this.FinishAffinity();
        // Ugly ugly ugly. This is done to cleanup the Delsys pipeline. Without the following line, it is going to crash every second start
        //Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
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
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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


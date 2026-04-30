using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using GhostlyGame;
using GhostlyGame.Models;

namespace GhostlyLib;

[Activity(Theme = "@style/Maui.SplashTheme",
    ScreenOrientation = ScreenOrientation.SensorLandscape,
    MainLauncher = false,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class GhostlyGameActivity : AndroidGameActivity
{
    private const int REQUEST_PERMISSION_CODE = 1001;
    public static GhostlyGameActivity Instance { get; private set; }

    private GhostlyLib.GhostlyGame _game;
    private Android.Views.View? _view;

    protected override void OnCreate(Bundle bundle)
    {
        this.Window.AddFlags(WindowManagerFlags.Fullscreen);
        this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
        base.OnCreate(bundle);

        InitPermission();

        EnableImmersiveMode();

        _game = new GhostlyLib.GhostlyGame();
        _game.RequestTherapistLogin = OpenPasswordDialog;
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
    }

    public void OpenPasswordDialog(Action<bool> onOk)
    {
        RunOnUiThread(() =>
        {
            // Dialog builder
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle(LocalizationResourceManager.Instance["EnterPassword"].ToString());

            // Root layout
            LinearLayout layout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };
            layout.SetPadding(50, 40, 50, 10);

            // Password input
            EditText passwordInput = new EditText(this)
            {
                Hint = LocalizationResourceManager.Instance["EnterPassword"].ToString()
            };

            passwordInput.InputType =
                InputTypes.ClassText | InputTypes.TextVariationPassword;

            layout.AddView(passwordInput,
                new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent));

            builder.SetView(layout);

            // OK button
            builder.SetPositiveButton(LocalizationResourceManager.Instance["Ok"].ToString(), async (s, e) =>
            {
                string storedUsername = await SecureStorage.Default.GetAsync("username");
                onOk?.Invoke(await GameSessionInfo.Instance.Uploader.SignIn(storedUsername, passwordInput.Text));
            });

            // Cancel button
            builder.SetNegativeButton(LocalizationResourceManager.Instance["Cancel"].ToString(), (s, e) =>
            {
                onOk?.Invoke(false);
            });

            // Optional: prevent dismiss on outside tap
            builder.SetCancelable(false);

            builder.Show();
        });
    }

    private void InitPermission()
    {
        List<string> mPermissionList = new List<string>();
        // When the Android version is 12 or greater, apply for new Bluetooth permissions
        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            mPermissionList.Add(Manifest.Permission.BluetoothScan);
            mPermissionList.Add(Manifest.Permission.BluetoothAdvertise);
            mPermissionList.Add(Manifest.Permission.BluetoothConnect);
            //Request for location permissions based on your actual needs
            mPermissionList.Add(Manifest.Permission.AccessCoarseLocation);
            mPermissionList.Add(Manifest.Permission.AccessFineLocation);
            mPermissionList.Add(Manifest.Permission.ReadExternalStorage);
            mPermissionList.Add(Manifest.Permission.ManageExternalStorage);
        }
        else
        {
            mPermissionList.Add(Manifest.Permission.AccessCoarseLocation);
            mPermissionList.Add(Manifest.Permission.AccessFineLocation);
            mPermissionList.Add(Manifest.Permission.ReadExternalStorage);
            mPermissionList.Add(Manifest.Permission.ManageExternalStorage);
        }

        ActivityCompat.RequestPermissions(this, mPermissionList.ToArray(), REQUEST_PERMISSION_CODE);
    }
}
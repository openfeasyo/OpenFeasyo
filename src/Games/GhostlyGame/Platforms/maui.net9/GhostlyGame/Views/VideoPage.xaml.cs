#if ANDROID
using Android.OS;
using Environment = Android.OS.Environment;
# endif
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Views;

namespace GhostlyGame.Views;

public partial class VideoPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;

    //private string videoFileName = "GhostlyGame.mp4";

    private string bfrBandsVideoNL = "De BFR banden GHOSTLY+ gebruik NL.mp4";
    private string bfrAppVideoNL = "De BFR app GHOSTLY+ gebruik NL.mp4";
    private string emgSensorsNL = "sEMG-sensoren plaatsing GHOSTLY+ gebruik NL .mp4";

    private string bfrBandsVideoFR = "De BFR banden GHOSTLY+ gebruik FR.mp4";
    private string bfrAppVideoFR = "De BFR app GHOSTLY+ gebruik FR.mp4";
    private string emgSensorsFR = "sEMG-sensoren plaatsing GHOSTLY+ gebruik FR.mp4";

    private string currentVideoFile = "";

    //private string videoUrl = "https://vub-my.sharepoint.com/:v:/g/personal/katarina_kostkova_vub_be/IQAcoWoAcUaHRbTs2il1IvSHAZqAj7_HRY_Smek1bzktF_E?nav=eyJyZWZlcnJhbEluZm8iOnsicmVmZXJyYWxBcHAiOiJPbmVEcml2ZUZvckJ1c2luZXNzIiwicmVmZXJyYWxBcHBQbGF0Zm9ybSI6IldlYiIsInJlZmVycmFsTW9kZSI6InZpZXciLCJyZWZlcnJhbFZpZXciOiJNeUZpbGVzTGlua0NvcHkifX0&e=SF5czl"; // your fixed URL

    /*public const string DIR_SEPARATOR =
#if ANDROID
            "/";
#else
            "\\";
#endif

    public const string HOME_FOLDER = "OpenFeasyo";*/

    public VideoPage()
    {
        InitializeComponent();
        BindingContext = this;

#if ANDROID
        //CopyVideoFilesIfNotExist();
#endif
    }

/*    private async void CopyVideoFilesIfNotExist()
    {
#if ANDROID

        var status = await Permissions.RequestAsync<Permissions.StorageRead>();
        if (status != PermissionStatus.Granted)
        {
            throw new Exception("Storage read permission denied");
        }
        
        //source
        //storage/emulated/0/Download/OpenFeasyo/videos
        string downloadDir = Android.OS.Environment.DirectoryDownloads;
        string OpenFeasyoVideosDownloadDir = downloadDir + DIR_SEPARATOR + HOME_FOLDER + DIR_SEPARATOR + "videos" + DIR_SEPARATOR;
        var videoDownloadsFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(OpenFeasyoVideosDownloadDir);

        //System.Diagnostics.Debug.WriteLine("videoDownloadsFolder: " + videoDownloadsFolder);

        
        //destination
        //storage/emulated/0/Android/data/org.openfeasyo.ghostlygame/files/
        var dir = Android.App.Application.Context.GetExternalFilesDir(null);
        var dest = Path.Combine(dir.AbsolutePath, "videos", currentVideoFile);

        //System.Diagnostics.Debug.WriteLine("dest: " + dest);


        if (!File.Exists(dest))
        {
            //File.Copy(videoDownloadsFolder + bfrBandsVideoFR, dest, overwrite: true);
            using var input = File.OpenRead(videoDownloadsFolder + DIR_SEPARATOR + bfrBandsVideoFR);
            using var output = File.Create(dest);

            await input.CopyToAsync(output);
        }

        dest = Path.Combine(FileSystem.AppDataDirectory, "videos", bfrAppVideoFR);

        if (!File.Exists(dest))
        {
            File.Copy(videoDownloadsFolder + bfrAppVideoFR, dest, overwrite: true);
        }

        dest = Path.Combine(FileSystem.AppDataDirectory, "videos", emgSensorsFR);

        if (!File.Exists(dest))
        {
            File.Copy(videoDownloadsFolder + emgSensorsFR, dest, overwrite: true);
        }

        dest = Path.Combine(FileSystem.AppDataDirectory, "videos", bfrBandsVideoNL);

        if (!File.Exists(dest))
        {
            File.Copy(videoDownloadsFolder + bfrBandsVideoNL, dest, overwrite: true);
        }

        dest = Path.Combine(FileSystem.AppDataDirectory, "videos", bfrAppVideoNL);

        if (!File.Exists(dest))
        {
            File.Copy(videoDownloadsFolder + bfrAppVideoNL, dest, overwrite: true);
        }

        dest = Path.Combine(FileSystem.AppDataDirectory, "videos", emgSensorsNL);

        if (!File.Exists(dest))
        {
            File.Copy(videoDownloadsFolder + emgSensorsNL, dest, overwrite: true);
        }
#endif
    }*/

    private async void LoadVideoAsync()
    {
#if ANDROID
        var status = await Permissions.RequestAsync<Permissions.StorageRead>();
        if (status != PermissionStatus.Granted)
        {
            throw new Exception("Storage read permission denied");
        }

        var dir = Android.App.Application.Context.GetExternalFilesDir(null);
        var path = Path.Combine(dir.AbsolutePath, "videos", currentVideoFile);

        //System.Diagnostics.Debug.WriteLine("DIR: " + path);

        // Play the video
        mediaPlayer.IsVisible = true;
        mediaPlayer.Source = path; //MediaSource.FromResource(currentVideoFile);
        mediaPlayer.Play();
#else
        //        // For Windows or other platforms, use AppData
        //var dir = FileSystem.AppDataDirectory;
#endif
    }

    private async void OnBfrBandsVideo(object sender, EventArgs e)
    {
        string storedLanguage = await SecureStorage.Default.GetAsync("language");
        if (storedLanguage == "fr-FR")
        {
            this.currentVideoFile = bfrBandsVideoFR;
        }
        else
        {
            this.currentVideoFile = bfrBandsVideoNL;
        }

        LoadVideoAsync();
    }

    private async void OnBfrAppVideo(object sender, EventArgs e)
    {
        //check language
        string storedLanguage = await SecureStorage.Default.GetAsync("language");
        if (storedLanguage == "fr-FR")
        {
            this.currentVideoFile = bfrAppVideoFR;
        }
        else
        {
            this.currentVideoFile = bfrAppVideoNL;
        }

        LoadVideoAsync();
    }

    private async void OnEmgVideo(object sender, EventArgs e)
    {
        string storedLanguage = await SecureStorage.Default.GetAsync("language");
        if (storedLanguage == "fr-FR")
        {
            this.currentVideoFile = emgSensorsFR;
        }
        else
        {
            this.currentVideoFile = emgSensorsNL;
        }

        LoadVideoAsync();
    }

    private async void OnOKClicked(object sender, EventArgs e)
    {
        //TODO navigate back (to login page)
        await Shell.Current.GoToAsync("..");
    }
}
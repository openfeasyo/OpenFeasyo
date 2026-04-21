#if ANDROID
using Android.OS;
using Environment = Android.OS.Environment;
# endif
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui;

namespace GhostlyGame.Views;

public partial class VideoPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;

    private string videoFileName = "GhostlyGame.mp4";
    private string videoUrl = "https://vub-my.sharepoint.com/:v:/g/personal/katarina_kostkova_vub_be/IQAcoWoAcUaHRbTs2il1IvSHAZqAj7_HRY_Smek1bzktF_E?nav=eyJyZWZlcnJhbEluZm8iOnsicmVmZXJyYWxBcHAiOiJPbmVEcml2ZUZvckJ1c2luZXNzIiwicmVmZXJyYWxBcHBQbGF0Zm9ybSI6IldlYiIsInJlZmVycmFsTW9kZSI6InZpZXciLCJyZWZlcnJhbFZpZXciOiJNeUZpbGVzTGlua0NvcHkifX0&e=SF5czl"; // your fixed URL


    public VideoPage()
    {
        InitializeComponent();
        BindingContext = this;

        LoadVideoAsync();
    }

    private async void LoadVideoAsync()
    {
#if ANDROID
        var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        if (status != PermissionStatus.Granted)
            throw new Exception("Storage permission denied");

        var dir = Android.App.Application.Context.GetExternalFilesDir(null);
        string folderPath = Path.Combine(dir.AbsolutePath, "videos");
        Directory.CreateDirectory(folderPath);
#else
        // For Windows or other platforms, use AppData
        string folderPath = FileSystem.AppDataDirectory;
#endif

        string localPath = Path.Combine(folderPath, videoFileName);

        if (!File.Exists(localPath))
        {
            statusLabel.Text = "Downloading video..." + videoUrl;
            try
            {
                using var client = new HttpClient();
                var data = await client.GetByteArrayAsync(videoUrl);
                await File.WriteAllBytesAsync(localPath, data);
                statusLabel.Text = "";// "Video downloaded!";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Download failed: {ex.Message}";
                return;
            }
        }
        else
        {
            //statusLabel.Text = "Playing cached video...";
        }

        // Play the video
        mediaPlayer.Source = localPath;
        mediaPlayer.Play();
    }

    private async void OnOKClicked(object sender, EventArgs e)
    {
        //TODO navigate back (to login page)
        await Shell.Current.GoToAsync("..");
    }

    private async void OnWatchOnline(object sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://www.youtube.com/watch?v=uf3yFalBmOw");
    }
}
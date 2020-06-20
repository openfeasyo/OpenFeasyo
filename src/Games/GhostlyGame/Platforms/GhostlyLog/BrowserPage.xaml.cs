using GhostlyLog.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using Vub.Etro.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GhostlyLog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrowserPage : ContentPage
    {
        public const string HOME_FOLDER = "OpenFeasyo";
        public static string HomeLocation = "";
        public const string DIR_SEPARATOR =
#if ANDROID
            "/";
#else
            "\\";
#endif

        public string FilesPath
        {
            get; set;
        }

        public delegate void NewPageRequested(ContentPage page);
        public event NewPageRequested DetailedPageRequested;
        private void OnDetailedPageRequested(ContentPage page) {
            NewPageRequested handler = DetailedPageRequested;
            handler?.Invoke(page);
        }

        /// <summary>
        /// This constructor is created only for the designer mode in Visual Studio
        /// </summary>
        public BrowserPage() {
            InitializeComponent();
            List<C3dFile> c3dFiles = new List<C3dFile>();
            var c3d = new C3dFile();
            c3d.Date = DateTime.Now;
            c3d.Image = "muscle.png";
            c3d.Level = "42";
            c3dFiles.Add(c3d);
            lstView.ItemsSource = c3dFiles;
        }

        public BrowserPage(string appVersion)
        {
            FilesPath = CheckAndCreateHomeFolder() + DIR_SEPARATOR + "Default";
            var o = this;
            InitializeComponent();
            header.Text = "Path: " + FilesPath;
            List<C3dFile> c3dFiles = new List<C3dFile>();
            foreach (string file in Directory.EnumerateFiles(FilesPath))
            {
                var fi = new FileInfo(file);

                var c3d = new C3dFile();
                c3d.Date = fi.LastWriteTime;
                c3d.Image = "muscle.png";
                c3d.Level = GetLevel(file);
                c3d.Path = file;
                c3dFiles.Add(c3d);
            }
            c3dFiles.Sort((q, p) => p.Date.CompareTo(q.Date));

            lstView.ItemsSource = c3dFiles;
            
        }

        void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                OnDetailedPageRequested(new DetailedReportPage(e.SelectedItem as C3dFile));
            }
        }

        private string GetLevel(string c3dFilePath)
        {
            C3dReader reader = new C3dReader();
            string strLevel = "?";
            if (reader.Open(c3dFilePath))
            {
                try
                {
                    Int16 level = reader.GetParameter<Int16>("INFO:GAME_LEVEL");
                    strLevel = "Level: " + level.ToString();
                }
                catch (Exception)
                {

                }
                finally
                {
                    reader.Close();
                }
            }
            return strLevel;
        }

        public static string CheckAndCreateHomeFolder()
        {

            String PersonalFolder = HomeLocation != "" ? HomeLocation :
#if ANDROID
            Xamarin.Essentials.FileSystem.AppDataDirectory;
#else
            Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif

            if (!Directory.Exists(PersonalFolder + DIR_SEPARATOR + HOME_FOLDER + DIR_SEPARATOR + "Patients"))
            {
                Directory.CreateDirectory(PersonalFolder + DIR_SEPARATOR + HOME_FOLDER + DIR_SEPARATOR + "Patients");
                Directory.CreateDirectory(PersonalFolder + DIR_SEPARATOR + HOME_FOLDER + DIR_SEPARATOR + "Patients" + DIR_SEPARATOR + "Default");
            }
            return PersonalFolder + DIR_SEPARATOR + HOME_FOLDER + DIR_SEPARATOR + "Patients";
        }
    }
}
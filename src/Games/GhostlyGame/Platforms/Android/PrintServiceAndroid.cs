using Android.Content;
using Android.Print;
using GhostlyGame;
using GhostlyLog;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(PrintServiceAndroid))]
namespace GhostlyGame
{
    
    class PrintServiceAndroid : IPrintService
    {
        public PrintServiceAndroid()
        {
        }
        public void Print(WebView viewToPrint, string fileName)
        {
            var adroidViewToPrint = Platform.CreateRenderer(viewToPrint).ViewGroup.GetChildAt(0) as Android.Webkit.WebView;

            if (adroidViewToPrint != null)
            {
                // Only valid for API 19+
                var version = Android.OS.Build.VERSION.SdkInt;

                if (version >= Android.OS.BuildVersionCodes.Kitkat)
                {
                    var printMgr = (PrintManager)Forms.Context.GetSystemService(Context.PrintService);
                    printMgr.Print("OpenFeasyo report", adroidViewToPrint.CreatePrintDocumentAdapter(fileName), null);
                }
            }
        }
    }

}
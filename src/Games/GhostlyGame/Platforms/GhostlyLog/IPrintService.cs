using Xamarin.Forms;

namespace GhostlyLog
{
    public interface IPrintService
    {
        void Print(WebView viewToPrint, string fileName);
    }
}

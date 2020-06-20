using GhostlyGame;
using GhostlyLog;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrlAndroid))]
namespace GhostlyGame
{
    class BaseUrlAndroid : IBaseUrl
    {
        public string Get()
        {
            return "file:///android_asset/";
        }
    }
}
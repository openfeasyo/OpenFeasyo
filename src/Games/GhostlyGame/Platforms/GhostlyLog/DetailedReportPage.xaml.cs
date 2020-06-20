using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GhostlyLog
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetailedReportPage : ContentPage
	{
        private ReportGenerator generator;

        private string _fileName = "";

		public DetailedReportPage ()
		{
            InitializeComponent ();
            var htmlSource = new HtmlWebViewSource();
            htmlSource.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
            htmlSource.Html = @"<html><style type=""text/css""> 
                @font-face { 
                    font-family: Vitamin;
                    src: url(""file:///android_asset/fonts/MyFontName.ttf"") 
                }
                body {
                    font-family: Vitamin;
                    font-size: medium;
                    text-align: justify;
                }</style>
                <body>
                    <h1>Xamarin.Forms</h1>
                    <p>Welcome to WebView.</p>
                </body></html>";
            webView.Source = htmlSource;
        }

        public DetailedReportPage(ViewModel.C3dFile file)
        {
            InitializeComponent();

            _fileName = file.Path;

            generator = new ReportGenerator();
            generator.C3dFile = file.Path;
            var assembly = Assembly.GetCallingAssembly();
            string templateName = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith("report_template.svg"));
            var htmlSource = new HtmlWebViewSource();
            htmlSource.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
            htmlSource.Html =
                 @"<html><style type=""text/css""> 
                @font-face { 
                    font-family: Vitamin;
                    src: url(""file:///android_asset/Fonts/vitamin.ttf"") 
                }
                </style>
                <body>" + 
                generator.RunTemplating(assembly.GetManifestResourceStream(templateName)) +
                "</body></html>";
            webView.Source = htmlSource;
        }

        private void Export_Clicked(object sender, EventArgs e)
        {
            string fileName = _fileName.Substring(0, _fileName.Length - 3) + "pdf"; 
            //var printMgr = (PrintManager)GetSystemService(MainActivity.PrintService);
            //printMgr.Print("MyPrintJob", webView.CreatePrintDocumentAdapter(fileName), new PrintAttributes.Builder().Build());

        }
    }
}
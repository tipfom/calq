using Calq.Core;
using Xamarin.Forms;

namespace Calq
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Term.PlatformPythonProvider = PythonWebProvider;
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

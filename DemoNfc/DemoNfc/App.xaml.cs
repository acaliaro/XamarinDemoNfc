using DemoNfc.Page;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DemoNfc
{
    public partial class App : Application
    {

        bool _stopTimer = false;

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts

            StartTimer();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            StopTimer();
        
        }

        private void StopTimer()
        {
            _stopTimer = true;
        }

        protected override void OnResume()
        {
            // Handle when your app resumes

            StartTimer();
        }

        void StartTimer()
        {

            _stopTimer = false;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {

                MessagingCenter.Send<App>(this, "Timer");
                return !_stopTimer; // true; // True = Repeat again, False = Stop the timer
            });

        }
    }
}

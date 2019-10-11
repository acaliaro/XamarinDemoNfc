using DemoNfc.Interface;
using DemoNfc.Page;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace DemoNfc.ViewModel
{
    public class ReadViewModel : INotifyPropertyChanged
    {

        public string TagId { get; set; }

        private List<string> _arg;

        public string ReceivedAt { get; set; }

        bool _isTapped;

        public ReadViewModel()
        {

            ImageTapCommand = new Command(async () => {


                try
                {

                    if (_isTapped)
                        return;

                    _isTapped = true;

                    if (Device.RuntimePlatform != Device.iOS)
                        MessagingCenter.Send<App, List<string>>((App)Xamarin.Forms.Application.Current, "Tag", new List<string> { "Test" });
                    else
                        DependencyService.Get<INfc>().StartSession();
                    _isTapped = false;

                }
                catch(Exception ex)
                {
                    _isTapped = false;
                    await Application.Current.MainPage.DisplayAlert(AppResources.Attenzione, ex.Message, AppResources.Ok);

                }
            });

            PopupCommand = new Command(async () =>
            {

                // If there is a popup, I close it
                if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Count > 0)
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new ResultPopupPage(_arg));

            });

            OnAppearingCommand = new Command( () => {

                MessagingCenter.Subscribe<App, List<string>>(this, "Tag", (sender, arg) =>
                {
                    TagId = arg[0]; // Pos 0 = TagID
                    _arg = arg;
                    DateTime dateTime = DateTime.Now;
                    ReceivedAt = dateTime.ToLongDateString() + "\n" +  dateTime.ToLongTimeString();
                    OnReceivedData?.Invoke(); //Now run the Action which, if it is not null, your ContentPage should have set to do the scrolling

                });

            });

            OnDisappearingCommand = new Command(() => {

                MessagingCenter.Unsubscribe<App, List<string>>(this, "Tag");

            });

        }


        public ICommand PopupCommand { get; protected set; }
        public ICommand ImageTapCommand { get; protected set; }
        public ICommand OnAppearingCommand { get; protected set; }
        public ICommand OnDisappearingCommand { get; protected set; } 
        public Action OnReceivedData { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

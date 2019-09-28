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
    public class MainViewModel : INotifyPropertyChanged
    {

        public string TagId { get; set; }
        public string ReceivedAt { get; set; }

        bool _isTapped;

        public MainViewModel()
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

            OnAppearingCommand = new Command( () => {

                MessagingCenter.Subscribe<App, List<string>>(this, "Tag", (sender, arg) =>
                {
                    OnTestAnimation?.Invoke(); //Now run the Action which, if it is not null, your ContentPage should have set to do the scrolling
                    TagId = arg[0]; // Pos 0 = TagID
                    DateTime dateTime = DateTime.Now;
                    ReceivedAt = dateTime.ToLongDateString() + "\n" +  dateTime.ToLongTimeString();

                    // If there is a popup, I close it
                    if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Count > 0)
                        Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();

                    Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new ResultPopupPage(arg));

                });

            });

            OnDisappearingCommand = new Command(() => {

                MessagingCenter.Unsubscribe<App, List<string>>(this, "Tag");

            });

        }



        public ICommand ImageTapCommand { get; protected set; }
        public ICommand OnAppearingCommand { get; protected set; }
        public ICommand OnDisappearingCommand { get; protected set; } 
        public Action OnTestAnimation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

using System;
using System.Collections.Generic;
using DemoNfc.ViewModel;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace DemoNfc.Page
{
    public partial class ResultPopupPage : PopupPage
    {
        public ResultPopupPage(List<string> arg)
        {
            this.BindingContext = new ResultPopupViewModel(arg);
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<App>(this, "Timer");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<App>(this, "Timer", (sender) => {
                
                if (--((ResultPopupViewModel)this.BindingContext).Seconds == 0)
                    Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
            
            });


            //int second = 8;
            //Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            //{

            //    ((ResultPopupViewModel)this.BindingContext).Seconds = second;

            //    second--;

            //    // Reached X seconds, I close the popup
            //    if(second < 0)
            //    {
            //        if(Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Count > 0)
            //            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
            //        return false;
            //    }

            //    return true; // True = Repeat again, False = Stop the timer
            //});
        }
    }
}

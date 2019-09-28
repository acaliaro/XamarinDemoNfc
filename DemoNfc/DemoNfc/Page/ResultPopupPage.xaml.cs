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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            int second = 8;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {

                ((ResultPopupViewModel)this.BindingContext).Seconds = second;

                second--;

                // Reached X seconds, I close the popup
                if(second < 0)
                {
                    Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
                    return false;
                }

                return true; // True = Repeat again, False = Stop the timer
            });
        }
    }
}

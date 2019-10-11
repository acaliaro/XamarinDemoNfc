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

        
        }
    }
}

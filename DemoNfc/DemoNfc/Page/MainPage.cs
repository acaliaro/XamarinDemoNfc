using Behaviors;
using DemoNfc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DemoNfc.Page
{
	public class MainPage : ContentPage
	{
		public MainPage ()
		{
            this.BindingContext = new MainViewModel();
            
            Image image = new Image() { Source = "baseline_nfc_black_48", VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand };

#if DEBUG
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, "ImageTapCommand");
            image.GestureRecognizers.Add(tapGestureRecognizer);
#endif

            Label labelTagId = new Label() { FontSize = 20, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Start };
            labelTagId.SetBinding(Label.TextProperty, new Binding("TagId", stringFormat: AppResources.TagIdFmt));

            Label labelReceivedAt = new Label() { FontSize = 20, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Center };
            labelReceivedAt.SetBinding(Label.TextProperty, "ReceivedAt");

            Label labelAvviso = new Label() { FontSize = 30, Text = AppResources.AvvicinaIlTagNfc, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.End };

            StackLayout stackLayout = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, Children = { labelTagId, labelReceivedAt, image, labelAvviso } };

            Frame frame = new Frame() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, BorderColor = Color.Red, CornerRadius = 10, Margin = Device.RuntimePlatform == Device.iOS ? new Thickness(10,20,10,10) : new Thickness(10), Content = stackLayout };            

            Content = frame;

            ((MainViewModel)this.BindingContext).OnTestAnimation = (async() =>
            {
                await frame.ScaleTo(0.9, 100);
                await frame.ScaleTo(1, 100);
            });

            InvokeCommandAction icaOnAppearing = new InvokeCommandAction();
            icaOnAppearing.SetBinding(InvokeCommandAction.CommandProperty, "OnAppearingCommand");
            EventHandlerBehavior ehbOnAppearing = new EventHandlerBehavior() { EventName = "Appearing" };
            ehbOnAppearing.Actions.Add(icaOnAppearing);

            InvokeCommandAction icaOnDisappearing = new InvokeCommandAction();
            icaOnDisappearing.SetBinding(InvokeCommandAction.CommandProperty, "OnDisappearingCommand");
            EventHandlerBehavior ehbOnDisappearing = new EventHandlerBehavior() { EventName = "Disappearing" };
            ehbOnDisappearing.Actions.Add(icaOnDisappearing);

            this.Behaviors.Add(ehbOnAppearing);
            this.Behaviors.Add(ehbOnDisappearing);


        }
    }
}
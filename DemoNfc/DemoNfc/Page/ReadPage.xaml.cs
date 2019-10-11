using DemoNfc.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DemoNfc.Page
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadPage : ContentPage
    {
        public ReadPage()
        {
            InitializeComponent();
            this.BindingContext = new ReadViewModel();

            ((ReadViewModel)this.BindingContext).OnReceivedData = (async () =>
            {
                // Do an animation
                await frame.ScaleTo(0.9, 100);
                await frame.ScaleTo(1, 100);

                // Visualize the popup for results
                ((ReadViewModel)this.BindingContext).PopupCommand.Execute(null);
            });
        }
    }
}
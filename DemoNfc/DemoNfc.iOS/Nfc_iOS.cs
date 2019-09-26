using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreNFC;
using DemoNfc.Interface;
using DemoNfc.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency (typeof (DemoNfc_iOS))]
namespace DemoNfc.iOS
{
    public class DemoNfc_iOS : INfc
    {
        public void StartSession()
        {
            AppDelegate.GetInstance().Scan();
        }
    }
}
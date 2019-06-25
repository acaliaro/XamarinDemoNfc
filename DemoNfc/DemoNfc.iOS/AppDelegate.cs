using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CoreNFC;
using CoreFoundation;

namespace DemoNfc.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, INFCNdefReaderSessionDelegate 
    {

        List<NFCNdefMessage> DetectedMessages = new List<NFCNdefMessage> { };

        public void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            foreach(NFCNdefMessage msg in messages)
            {
                DetectedMessages.Add(msg);
            }
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                //this.TableView.ReloadData();
            });
        }

        public void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            var readerError = (NFCReaderError)(long)error.Code;

            if (readerError != NFCReaderError.ReaderSessionInvalidationErrorFirstNDEFTagRead &&
                readerError != NFCReaderError.ReaderSessionInvalidationErrorUserCanceled)
            {

                var alertController = UIAlertController.Create("Session Invalidated", error.LocalizedDescription, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    //this.PresentViewController(alertController, true, null);
                });
            }

        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}

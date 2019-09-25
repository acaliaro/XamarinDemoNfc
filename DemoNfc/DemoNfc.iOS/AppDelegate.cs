using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CoreNFC;
using CoreFoundation;
using Xamarin.Forms;

namespace DemoNfc.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, INFCNdefReaderSessionDelegate 
    {

        private static AppDelegate appDelegate;

        List<NFCNdefMessage> DetectedMessages = new List<NFCNdefMessage> { };
        List<string> tags = new List<string>();

        public void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            tags.Clear();

            foreach(NFCNdefMessage msg in messages)
            {
                DetectedMessages.Add(msg);
                foreach(var payload in msg.Records)
                {
                    if(payload.TypeNameFormat == NFCTypeNameFormat.AbsoluteUri)
                    {

                    }
                }
            }
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {

                MessagingCenter.Send<App, List<string>>((App)Xamarin.Forms.Application.Current, "Tag", tags);

                //this.TableView.ReloadData();
            });
        }

        public static AppDelegate GetInstance()
        {
            return appDelegate;
        }

        public void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            var readerError = (NFCReaderError)(long)error.Code;

            System.Diagnostics.Debug.WriteLine("DidInvalidate: errorcode " + error.Code.ToString() + " " + error.LocalizedDescription + " " + error.LocalizedFailureReason);

            if (readerError != NFCReaderError.ReaderSessionInvalidationErrorFirstNDEFTagRead &&
                readerError != NFCReaderError.ReaderSessionInvalidationErrorUserCanceled)
            {

                InvokeOnMainThread(() =>
                {
                    var alertController = UIAlertController.Create("Session Invalidated", error.LocalizedDescription, UIAlertControllerStyle.Alert);
                    alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        //this.PresentViewController(alertController, true, null);
                    });
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

            appDelegate = this;

            return base.FinishedLaunching(app, options);
        }

        internal void StartSession()
        {
            var Session = new NFCNdefReaderSession(this, null, true);
            //Session.AlertMessage = "You can hold you NFC-tag to the back-top of your iPhone";
            Session?.BeginSession();
        }
    }
}

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
    public class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, INFCNdefReaderSessionDelegate
    {

        private static AppDelegate appDelegate;


        public static AppDelegate GetInstance()
        {
            return appDelegate;
        }



        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            appDelegate = this;

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        List<NFCNdefMessage> DetectedMessages = new List<NFCNdefMessage> { };
        NFCNdefReaderSession Session;
        string CellIdentifier = "reuseIdentifier";

        public void Scan()
        {
            ObjCRuntime.Class.ThrowOnInitFailure = false;

            //if (NFCNdefReaderSession.ReadingAvailable)
            //{
                Session = new NFCNdefReaderSession(this, null, true);
                if (Session != null)
                {
                    Session.AlertMessage = "You can hold you NFC-tag to the back-top of your iPhone";
                    Session.BeginSession();
                }
            //}
        }

        #region NFCNDEFReaderSessionDelegate

        public void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {

            List<string> tags = new List<string>();


            foreach (NFCNdefMessage msg in messages)
            {
                DetectedMessages.Add(msg);

                foreach(NFCNdefPayload payload in msg.Records)
                {
                    switch(payload.TypeNameFormat)
                    {
                        case NFCTypeNameFormat.NFCWellKnown:

                            break;
                        default:
                            break;
                    }
                    tags.Add(payload.TypeNameFormat + " - " + payload.Payload + " - " + payload.Type + " - " + payload.Identifier);
                }

                MessagingCenter.Send<App, List<string>>((App)Xamarin.Forms.Application.Current, "Tag", tags);

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
                InvokeOnMainThread(() => {
                    var alertController = UIAlertController.Create("Session Invalidated", error.LocalizedDescription, UIAlertControllerStyle.Alert);
                    alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        //this.PresentViewController(alertController, true, null);
                    });
                });

            }

        }

        #endregion


    }
}

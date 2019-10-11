using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CoreNFC;
using CoreFoundation;
using Xamarin.Forms;
using System.IO;
using Matcha.BackgroundService.iOS;

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

            Rg.Plugins.Popup.Popup.Init();

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            appDelegate = this;
            BackgroundAggregator.Init(this);
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

                    NSString typeString = payload.Type.ToString(NSStringEncoding.UTF8);
                    NSString identifierString = payload.Identifier.ToString(NSStringEncoding.UTF8);
                    var payloadLength = (int) payload.Payload.Length;

                    switch (payload.TypeNameFormat)
                    {
                        case NFCTypeNameFormat.NFCWellKnown:

                            if (payloadLength < 1)
                                return;


                            if (typeString == "T")
                            {

                                ParseTextPayload(payload.Payload.ToArray(), payload.Payload.Length, out string langCode, out string text);

                                tags.Add(typeString + " - " + langCode + " - " + text);

                            }
                            else if (typeString == "U")
                            {

                                ParseUriPayload(payload.Payload.ToArray(), payload.Payload.Length, out string text);

                                tags.Add(typeString + " - " + text);

                            }
                            else if(typeString == "Sp")
                            {

                                int length =(int) payload.Payload.Length;
                                byte[] bytes = payload.Payload.ToArray();
                                int index = 0;

                                while (true)
                                {


                                    var statusByte = payload.Payload[index++];
                                    var headerIsMessageBegin = statusByte & 0x80;
                                    var headerIsMessageEnd = statusByte & 0x40;
                                    var headerIsChunkedUp = statusByte & 0x20;
                                    var headerIsShortRecord = statusByte & 0x10;
                                    var headerIsIdentifierPresent = statusByte & 0x08;
                                    var headerTypeNameFormatCode = statusByte & 0x07;
                                    int headerPayloadLength = 0;
                                    byte headerIdentifier = 0;

                                    if (index + 1 > payloadLength)
                                        break;

                                    var typeLength = payload.Payload[index++];

                                    if ((headerIsShortRecord != 0 && (index + 1) > payloadLength) || (headerIsShortRecord == 0 && index + 4 > payloadLength))
                                        break;

                                    if (headerIsShortRecord != 0)
                                    {
                                        headerPayloadLength = payload.Payload[index];
                                        index += 1;
                                    }
                                    else
                                    {
                                        byte[] b = new byte[4];
                                        Array.Copy(payload.Payload.ToArray(), index, b, 0, 4);
                                        headerPayloadLength = BitConverter.ToInt32(b, 0);

                                        index += 4;
                                    }

                                    int identifierLength = 0;

                                    if(headerIsIdentifierPresent != 0)
                                    {
                                        if (index + 1 > payloadLength)
                                            break;

                                        identifierLength = payload.Payload[index++];
                                    }

                                    if (index + typeLength > payloadLength)
                                        break;

                                    NSString headerType = (Foundation.NSString)NSData.FromStream(new MemoryStream(bytes, index, typeLength)).ToString(NSStringEncoding.UTF8);
                                    if (string.IsNullOrEmpty(headerType))
                                        break;

                                    index += typeLength;

                                    if(identifierLength > 0)
                                    {
                                        if (index + 1 > payloadLength)
                                            break;

                                        headerIdentifier = payload.Payload[index];
                                        index += identifierLength;

                                    }

                                    var headerPayloadOffset = index;

                                    length -= headerPayloadOffset;
                                    byte[] newPayloadArray = new byte[headerPayloadLength];
                                    Array.Copy(payload.Payload.ToArray(), headerPayloadOffset, newPayloadArray, 0, headerPayloadLength);

                                    if (headerType == "U")
                                    {
                                        ParseUriPayload(newPayloadArray, (System.nuint)headerPayloadLength, out string text);
                                        tags.Add(headerType + " - " + text);

                                    }
                                    else if(headerType == "T")
                                    {
                                        ParseTextPayload(newPayloadArray, (System.nuint)headerPayloadLength, out string langCode, out string text);

                                        tags.Add(headerType + " - " + langCode + " - " + text);

                                    }

                                    length -= headerPayloadLength;
                                    if (headerIsMessageEnd != 0 || length == 0)
                                        break;

                                    index += headerPayloadLength;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                
                MessagingCenter.Send<App, List<string>>((App)Xamarin.Forms.Application.Current, "Tag", tags);

            }
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                //this.TableView.ReloadData();
            });
        }

        private void ParseTextPayload(byte[] payload, nuint length, out string langCode, out string text)
        {

            var isUTF16 = payload[0] & 0x80;
            var codeLength = payload[0] & 0x7f;
            langCode = "";
            text = "";

            if ((int)length < 1 + codeLength)
                return;

            langCode = (Foundation.NSString)NSData.FromStream(new MemoryStream(payload, 1, codeLength)).ToString();
            text = (Foundation.NSString)NSData.FromStream(new MemoryStream(payload, 1 + codeLength, (int)length - 1 - codeLength)).ToString(isUTF16 == 0 ? NSStringEncoding.UTF8 : NSStringEncoding.UTF16BigEndian);


        }

        private void ParseUriPayload(byte[] payload, nuint length, out string text)
        {

            var code = payload[0];
            NSString originalText = (Foundation.NSString)NSData.FromStream(new MemoryStream(payload, 1, (int)length - 1)).ToString(NSStringEncoding.UTF8);
            text = (Foundation.NSString)DemoNfc.Utility.Utility.UriMap(code);
            //switch (code)
            //{
            //    case 0x00:
            //        text = originalText;
            //        break;
            //    case 0x01:
            //        text = (Foundation.NSString)("http://www." + originalText);
            //        break;
            //    case 0x02:
            //        text = (Foundation.NSString)("https://www." + originalText);
            //        break;
            //    case 0x03:
            //        text = (Foundation.NSString)("http://" + originalText);
            //        break;
            //    default:
            //        text = originalText;
            //        break;
            //}
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

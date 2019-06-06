using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Nfc;
using Android.Content;
using System.Text;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections;

namespace DemoNfc.Droid
{
    [Activity(Label = "DemoNfc", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        private NfcAdapter _nfcAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (_nfcAdapter == null)
            {
                var alert = new AlertDialog.Builder(this).Create();
                alert.SetMessage("NFC is not supported on this device.");
                alert.SetTitle("NFC Unavailable");
                alert.Show();
            }
            else
            {
                var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
                var ndefDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
                var techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);

                var filters = new[] { ndefDetected, tagDetected, techDetected };

                var intent = new Intent(this, this.GetType()).AddFlags(ActivityFlags.SingleTop);

                var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

                _nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (_nfcAdapter != null)
                _nfcAdapter.DisableForegroundDispatch(this);
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent.Action == NfcAdapter.ActionTagDiscovered)
            {

                List<string> tags = new List<string>();

                var id = intent.GetByteArrayExtra(NfcAdapter.ExtraId);

                if (id != null)
                {
                    string data = "";
                    for (int ii = 0; ii < id.Length; ii++)
                    {
                        if (!string.IsNullOrEmpty(data))
                            data += "-";
                        data += id[ii].ToString("X2");
                    }

                    tags.Add(data);

                }
                else
                    tags.Add(null);

                var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
                if (tag != null)
                {

                    var rawTagMessages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraTag);

                    // First get all the NdefMessage
                    var rawMessages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
                    if (rawMessages != null)
                    {
                        var msg = (NdefMessage)rawMessages[0];

                        // Get NdefRecord which contains the actual data
                        var record = msg.GetRecords()[0];
                        if (record != null)
                        {
                            if (record.Tnf == NdefRecord.TnfWellKnown) // The data is defined by the Record Type Definition (RTD) specification available from http://members.nfc-forum.org/specs/spec_list/
                            {
                                // Get the transfered data
                                var payload = record.GetPayload();

                                BitArray bitArray = new BitArray(payload[0]); 

                                //string textEncoding = ((payload[0] & 0200) == 0) ? "UTF-8" : "UTF-16";
                                //int languageCodeLength = payload[0] & 0077;

                                string code = "";

                                for (int ii = 1; ii < payload.Length - 1; ii++)
                                    code += (char)payload[ii];

//                                var data = Encoding.ASCII.GetString(record.GetPayload());

                                System.Diagnostics.Debug.WriteLine("Read data: " + code);

                                tags.Add(code);
                            }
                        }
                    }
                }

                MessagingCenter.Send<App, List<string>>((App)Xamarin.Forms.Application.Current, "Tag", tags);

            }
            else if(intent.Action == NfcAdapter.ActionNdefDiscovered)
            {

            }
        }
    }
}
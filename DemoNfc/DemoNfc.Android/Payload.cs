using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DemoNfc.Droid
{
    public class Payload : IParsedNdefRecord
    {
        private NdefRecord record;

        public Payload(NdefRecord record)
        {
            this.record = record;
        }

        public string Str()
        {
            return System.Text.Encoding.Default.GetString(this.record.GetPayload());
        }
    }
}
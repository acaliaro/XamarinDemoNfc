using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Util;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace DemoNfc.Droid
{
    internal class UriRecord : IParsedNdefRecord
    {

        static UriRecord _uriRecord = new UriRecord();
        private readonly string _uri;

        public UriRecord(string uri)
        {
            this._uri = uri;
        }

        public UriRecord()
        {
        }

        public static UriRecord GetInstance()
        {
            return _uriRecord;
        }

        internal bool IsUri(NdefRecord record)
        {

            try
            {
                Parse(record);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public UriRecord Parse(NdefRecord record)
        {
            var tnf = record.Tnf;
            if (tnf == NdefRecord.TnfWellKnown)
                return ParseWellKnown(record);
            else if (tnf == NdefRecord.TnfAbsoluteUri)
                return ParseAbsolute(record);
            else
                throw new Exception("Unknown tnf");
        }

        private UriRecord ParseAbsolute(NdefRecord record)
        {
            /*
                byte[] payload = record.getPayload();
                Uri uri = Uri.parse(new String(payload, Charset.forName("UTF-8")));
                return new UriRecord(uri);
            */

            var payload = record.GetPayload();
            Encoding enc = Encoding.GetEncoding("UTF-8");
            string text = enc.GetString(payload);
            return new UriRecord( text);

        }

        private UriRecord ParseWellKnown(NdefRecord record)
        {
            //Preconditions.CheckArgument(Arrays.Equals(record.GetTypeInfo(), NdefRecord.RtdUri));

            if (record.GetTypeInfo()[0] != NdefRecord.RtdUri[0])
                throw new Exception();
            
            //                 Preconditions.checkArgument(Arrays.equals(record.getType(), NdefRecord.RTD_URI));
            //              byte[] payload = record.getPayload();
            /// *
            // * payload[0] contains the URI Identifier Code, per the
            // * NFC Forum "URI Record Type Definition" section 3.2.2.
            // *
            // * payload[1]...payload[payload.length - 1] contains the rest of
            // * the URI.
            // */
            //        String prefix = URI_PREFIX_MAP.get(payload[0]);
            //        byte[] fullUri =
            //            Bytes.concat(prefix.getBytes(Charset.forName("UTF-8")), Arrays.copyOfRange(payload, 1,
            //                payload.length));
            //        Uri uri = Uri.parse(new String(fullUri, Charset.forName("UTF-8")));
            //        return new UriRecord(uri);

            var payload = record.GetPayload();
            string prefix = DemoNfc.Utility.Utility.UriMap(payload[0]);
            Encoding enc = Encoding.GetEncoding("UTF-8");
            string text = enc.GetString(payload, 1, payload.Length - 1);
            return new UriRecord( prefix + text);
        }

        public string Str()
        {
            return this._uri;
        }
    }
}
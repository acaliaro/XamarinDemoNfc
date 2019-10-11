using System;
using System.Collections.Generic;
using System.Text;

namespace DemoNfc.Utility
{
    public static class Utility
    {
        static Dictionary<byte, string> MAP = new Dictionary<byte, string>()
        {
            {(byte) 0x00, "" },
            {(byte)  0x01, "http://www."},
            {(byte)  0x02, "https://www."},
            {(byte)  0x03, "http://"},
            {(byte)  0x04, "https://"},
            {(byte)  0x05, "tel:"},
            {(byte)  0x06, "mailto:"},
            {(byte)  0x07, "ftp://anonymous:anonymous@"},
            {(byte)  0x08, "ftp://ftp."},
            {(byte)  0x09, "ftps://"},
            {(byte)  0x0A, "sftp://"},
            {(byte)  0x0B, "smb://"},
            {(byte)  0x0C, "nfs://"},
            {(byte)  0x0D, "ftp://"},
            {(byte)  0x0E, "dav://"},
            {(byte)  0x0F, "news:"},
            {(byte)  0x10, "telnet://"},
            {(byte)  0x11, "imap:"},
            {(byte)  0x12, "rtsp://"},
            {(byte)  0x13, "urn:"},
            {(byte)  0x14, "pop:"},
            {(byte)  0x15, "sip:"},
            {(byte)  0x16, "sips:"},
            {(byte)  0x17, "tftp:"},
            {(byte)  0x18, "btspp://"},
            {(byte)  0x19, "btl2cap://"},
            {(byte)  0x1A, "btgoep://"},
            {(byte)  0x1B, "tcpobex://"},
            {(byte)  0x1C, "irdaobex://"},
            {(byte)  0x1D, "file://"},
            {(byte)  0x1E, "urn:epc:id:"},
            {(byte)  0x1F, "urn:epc:tag:"},
            {(byte)  0x20, "urn:epc:pat:"},
            {(byte)  0x21, "urn:epc:raw:"},
            {(byte)  0x22, "urn:epc:"},
            {(byte)  0x23, "urn:nfc:"}
        };

        public static string UriMap(byte code)
        {
            MAP.TryGetValue(code, out string valore);
            return valore;
        }
    }
}

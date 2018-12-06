using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace LanStatusCheck.Classes
{
    public class IpSegment
    {

        private UInt32 _ip;

        private UInt32 _mask;

        public IpSegment(string ip, string mask)
        {
            _ip = ip.ParseIp();
            _mask = mask.ParseIp();
        }

        public UInt32 NumberOfHosts
        {
            get { return ~_mask + 1; }
        }

        public UInt32 NetworkAddress
        {
            get { return _ip & _mask; }
        }

        public UInt32 BroadcastAddress
        {
            get { return NetworkAddress + ~_mask; }
        }

        public IEnumerable<UInt32> Hosts()
        {
            for (var host = NetworkAddress + 1; host < BroadcastAddress; host++)
            {
                yield return host;
            }
        }

    }

    public static class IpHelpers
    {
        public static string ToIpString(this UInt32 value)
        {
            var bitmask = 0xff000000;
            var parts = new string[4];
            for (var i = 0; i < 4; i++)
            {
                var masked = ( value & bitmask ) >> ( ( 3 - i ) * 8 );
                bitmask >>= 8;
                parts[i] = masked.ToString(CultureInfo.InvariantCulture);
            }
            return String.Join(".", parts);
        }

        public static UInt32 ParseIp(this string ipAddress)
        {
            var splitted = ipAddress.Split('.');
            UInt32 ip = 0;
            for (var i = 0; i < 4; i++)
            {
                ip = ( ip << 8 ) + UInt32.Parse(splitted[i]);
            }
            return ip;
        }

        public static bool ParseIp(string ipStr, out IPAddress ipAddress, out string error)
        {
            ipAddress = null;

            error = String.Empty;

            if (String.IsNullOrEmpty(ipStr))
            {
                return false;
            }

            try
            {
                ipAddress = IPAddress.Parse(ipStr);

                return true;
            }
            catch (Exception exp)
            {
                error = exp.Message;

                return false;
            }
        }
    }
}

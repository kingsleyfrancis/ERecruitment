using System;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace ERecruitment.Core.Common
{
    public static class ClientIp
    {
        private static readonly IpRange[] SPrivateRanges =
        {
            new IpRange("0.0.0.0", "2.255.255.255"),
            new IpRange("10.0.0.0", "10.255.255.255"),
            new IpRange("127.0.0.0", "127.255.255.255"),
            new IpRange("169.254.0.0", "169.254.255.255"),
            new IpRange("172.16.0.0", "172.31.255.255"),
            new IpRange("192.0.2.0", "192.0.2.255"),
            new IpRange("192.168.0.0", "192.168.255.255"),
            new IpRange("255.255.255.0", "255.255.255.255")
        };

        private static readonly HeaderItem[] SHeaderItems =
        {
            new HeaderItem("HTTP_CLIENT_IP", false),
            new HeaderItem("HTTP_X_FORWARDED_FOR", true),
            new HeaderItem("HTTP_X_FORWARDED", false),
            new HeaderItem("HTTP_X_CLUSTER_CLIENT_IP", false),
            new HeaderItem("HTTP_FORWARDED_FOR", false),
            new HeaderItem("HTTP_FORWARDED", false),
            new HeaderItem("HTTP_VIA", false),
            new HeaderItem("REMOTE_ADDR", false)
        };

        public static string ClientIpFromRequest(this HttpRequestBase request, bool skipPrivate)
        {
            foreach (HeaderItem item in SHeaderItems)
            {
                string ipString = request.Headers[item.Key];

                if (String.IsNullOrEmpty(ipString))
                    continue;

                if (item.Split)
                {
                    foreach (string ip in ipString.Split(','))
                        if (ValidIp(ip, skipPrivate))
                            return ip;
                }
                else
                {
                    if (ValidIp(ipString, skipPrivate))
                        return ipString;
                }
            }

            return request.UserHostAddress;
        }

        private static bool ValidIp(string ip, bool skipPrivate)
        {
            IPAddress ipAddr;

            ip = ip == null ? String.Empty : ip.Trim();

            if (0 == ip.Length
                || false == IPAddress.TryParse(ip, out ipAddr)
                || (ipAddr.AddressFamily != AddressFamily.InterNetwork
                    && ipAddr.AddressFamily != AddressFamily.InterNetworkV6))
                return false;

            if (skipPrivate && ipAddr.AddressFamily == AddressFamily.InterNetwork)
            {
                ulong addr = IpRange.AddrToUInt64(ipAddr);
                foreach (IpRange range in SPrivateRanges)
                {
                    if (range.Encompasses(addr))
                        return false;
                }
            }

            return true;
        }


        /// Describes a header item (key) and if it is expected to be 
        /// a comma-delimited string
        private sealed class HeaderItem
        {
            public readonly string Key;
            public readonly bool Split;

            public HeaderItem(string key, bool split)
            {
                Key = key;
                Split = split;
            }
        }

        /// Provides a simple class that understands how to parse and
        /// compare IP addresses (IPV4 and IPV6) ranges.
        private sealed class IpRange
        {
            private readonly UInt64 _end;
            private readonly UInt64 _start;

            public IpRange(string startStr, string endStr)
            {
                _start = ParseToUInt64(startStr);
                _end = ParseToUInt64(endStr);
            }

            public static UInt64 AddrToUInt64(IPAddress ip)
            {
                byte[] ipBytes = ip.GetAddressBytes();
                UInt64 value = 0;

                foreach (byte abyte in ipBytes)
                {
                    value <<= 8; // shift
                    value += abyte;
                }

                return value;
            }

            private static UInt64 ParseToUInt64(string ipStr)
            {
                IPAddress ip = IPAddress.Parse(ipStr);
                return AddrToUInt64(ip);
            }

            public bool Encompasses(UInt64 addrValue)
            {
                return _start <= addrValue && addrValue <= _end;
            }

            public bool Encompasses(IPAddress addr)
            {
                ulong value = AddrToUInt64(addr);
                return Encompasses(value);
            }
        };

        // order is in trust/use order top to bottom
    }
}
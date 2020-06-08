using System;

namespace _1xParser
{
    [Serializable]
    public class Params
    {
        public string telegToken = "Telegram API token";

        public bool useProxy = false;
        public string proxyIP = "0.0.0.0";
        public int proxyPort = 1234;

        public bool Equals(Params obj)
        {
            return telegToken == obj.telegToken
                && useProxy == obj.useProxy
                && proxyIP == obj.proxyIP
                && proxyPort == obj.proxyPort;
        }
    }
}

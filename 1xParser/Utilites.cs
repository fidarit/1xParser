using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace _1xParser
{
    static class Utilites
    {
        static readonly DateTime unixHelper = new DateTime(1970, 1, 1);
        static readonly object getHtmlLocker = new object();
        static readonly object getPostLocker = new object();

        public static int NowUNIX()
        {
            return (int)(DateTime.UtcNow - unixHelper).TotalSeconds;
        }

        public static string GetHTML(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 
                | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    lock (getHtmlLocker)
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.Encoding = Encoding.GetEncoding(1251);
                            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                            Stream data = client.OpenRead(url);
                            StreamReader reader = new StreamReader(data, Encoding.UTF8);
                            string line = reader.ReadToEnd();

                            data.Close();
                            reader.Close();

                            return line;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Thread.Sleep(1500);
                }
            }
            return null;
        }
        public static string Post(string url, string data)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            DateTime time = DateTime.Now;
            string Out = "";
            try
            {
                lock (getPostLocker)
                {
                    WebRequest req = WebRequest.Create(url);
                    if (Params.UseProxy)
                    {
                        req.Proxy = new WebProxy(Params.ProxyIP, Params.ProxyPort);
                        req.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    }
                    req.UseDefaultCredentials = true;
                    req.Method = "POST";
                    req.Timeout = 10000;
                    req.ContentType = "application/x-www-form-urlencoded"; 

                    byte[] sentData = Encoding.GetEncoding(1251).GetBytes(data);
                    req.ContentLength = sentData.Length;
                    Stream sendStream = req.GetRequestStream();
                    sendStream.Write(sentData, 0, sentData.Length);
                    sendStream.Close();

                    WebResponse res = req.GetResponse();
                    Stream ReceiveStream = res.GetResponseStream();

                    using (StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8))
                    {
                        char[] read = new char[256];
                        int count = sr.Read(read, 0, 256);
                        while (count > 0)
                        {
                            string str = new string(read, 0, count);
                            Out += str;
                            count = sr.Read(read, 0, 256);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (Out.Length < 10)
            {
                string s = string.Format("Что-то пошло не так с отправкой/получением данных: {0} {1}", url, data);
                Debug.LogWarning(s);
            }

            //К API Telegram разрешено обращаться примерно 30 раз в сек,
            //поэтому на всякий случай отправляем поток на сон на 35 мс
            int sleepTime = (int)(time.AddMilliseconds(35) - DateTime.Now).TotalMilliseconds;
            if (sleepTime > 0) Thread.Sleep(sleepTime);

            return Out;
        }

    }
}

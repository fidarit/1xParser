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
            for (int i = 0; i < 5; i++)
            {
                lock (getHtmlLocker)
                {
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.Encoding = Encoding.GetEncoding(1251);
                            //client.Encoding = Encoding.UTF8;
                            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                            Stream data = client.OpenRead(url);
                            StreamReader reader = new StreamReader(data, Encoding.UTF8);
                            string line = reader.ReadToEnd();
                            data.Close();
                            reader.Close();

                            return line;
                        }
                    }
                    catch (Exception e)
                    {
                        LogException(e);
                        Thread.Sleep(1500);
                    }
                }
            }
            return null;
        }
        public static string Post(string url, string data)
        {
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
                    //byte[] sentData = Encoding.UTF8.GetBytes(data);
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
                LogException(e);
            }
            return Out;
        }
        public static string GET(string url)
        {
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
                    req.Timeout = 10000;
                    WebResponse resp = req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    Out = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
            return Out;
        }

        public static void LogException(Exception e)
        {
            if (!(e.Message.Contains("Thread") || e.Message.Contains("Поток"))) { 
                LogError(e.StackTrace.Replace(" в ", Environment.NewLine + "\t в "));
                LogError(e.Message);
            }
        }
        public static void LogWarning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteLine(msg);
        }
        public static void LogError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(msg);
        }
        public static void Log(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine(msg);
        }

        static void WriteLine(string msg)
        {
            Console.WriteLine(DateTime.Now.ToShortDateString() + " "
                + DateTime.Now.ToLongTimeString() + " " + msg);
        }
    }
}

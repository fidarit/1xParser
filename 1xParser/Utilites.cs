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
        static readonly object getLocker = new object();

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
        public static void LogException(Exception e)
        {
            if(!e.StackTrace.Contains("Thread"))
                LogError(e.StackTrace.Replace(" в ", Environment.NewLine + "\t в "));
            LogError(e.Message);
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
        public static string GET(string url)
        {
            string Out = "";
            try
            {
                lock (getLocker)
                {
                    WebRequest req = WebRequest.Create(url);
                    req.Proxy = new WebProxy(Params.ProxyIP, Params.ProxyPort);
                    req.UseDefaultCredentials = true;
                    req.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    WebResponse resp = req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    Out = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch(Exception e)
            {
                LogException(e);
            }
            return Out;
        }
    }
}

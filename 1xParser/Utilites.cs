using System;
using System.IO;
using System.Net;
using System.Text;

namespace _1xParser
{
    static class Utilites
    {
        public static string GetHTML(string url)
        {
            string line;
            WebClient client = new WebClient();
            client.Encoding = Encoding.GetEncoding(1251);
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data, Encoding.UTF8);
            line = reader.ReadToEnd();
            data.Close();
            reader.Close();

            return line;
        }
        public static void cWarning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            writeL(msg);
        }
        public static void cError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            writeL(msg);
        }
        public static void cMsg(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            writeL(msg);
        }
        static void writeL(string msg)
        {
            Console.WriteLine(System.DateTime.Now.ToShortDateString() + " "
                + System.DateTime.Now.ToLongTimeString() + " " + msg);
        }
        public static string GET(string url)
        {
            string Out = "";
            try
            {
                WebRequest req = WebRequest.Create(url);
                req.Proxy = new WebProxy(Params.proxyIP, Params.proxyPort);
                req.UseDefaultCredentials = true;
                req.Proxy.Credentials = CredentialCache.DefaultCredentials;
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                Out = sr.ReadToEnd();
                sr.Close();
            }
            catch(Exception e)
            {
                Utilites.cError(e.Message);
            }
            return Out;
        }
    }
}

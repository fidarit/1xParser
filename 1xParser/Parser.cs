using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace _1xParser
{
    static class Parser
    {

        public static void ParseLine()
        {
            string fURL = "https://1xstavka.ru/line/Handball/";
            string mass = Utilites.GetHTML(fURL);
            jsonFormats.RootObj[] objs;

            Match match = Regex.Match(mass, @"<script[^>]*?>.*?SSR_DASHBOARD(.*?);.*?<\/script>");
            if (match.Success)
            {
                mass = match.Groups[1].Value; //значение скобки рег выр
            }

            match = Regex.Match(mass, @"Value.*?:(\[.*\])");
            if (match.Success)
            {
                mass = match.Groups[1].Value; //значение скобки рег выр
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            objs = serializer.Deserialize<jsonFormats.RootObj[]>(mass);
            //

            File.WriteAllText("123.txt", mass);
        }

        public static void ParseLive()
        {
            string fURL = "https://1xstavka.ru/live/Handball/";
            string mass = Utilites.GetHTML(fURL);
            jsonFormats.RootObj[] objs;

            Match match = Regex.Match(mass, @"<script[^>]*?>.*?SSR_DASHBOARD(.*?);.*?<\/script>");
            if (match.Success)
            {
                mass = match.Groups[1].Value; //значение скобки рег выр
            }

            match = Regex.Match(mass, @"Value.*?:(\[.*\])");
            if (match.Success)
            {
                mass = match.Groups[1].Value; //значение скобки рег выр
            }
            if (mass.Length < 10) return;
            //Utilites.cMsg(mass);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            objs = serializer.Deserialize<jsonFormats.RootObj[]>(mass);
            //

            File.WriteAllText("1234.txt", mass);
        }

    }
}

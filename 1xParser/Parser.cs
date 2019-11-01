using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace _1xParser
{
    static class Parser
    {
        private static DateTime utcHelper = new DateTime(1970, 1, 1);
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

            File.WriteAllText("1.txt", serializer.Serialize(Program.games));
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
            
            for(int i = 0; i < objs.Length; i++)
            {
                long id = objs[i].CI;
                if (Program.games.ContainsKey(id))
                {
                    
                }
                else
                {
                    Game resObj = new Game();
                    resObj.league = objs[i].L;
                    resObj.startTimeUTC = objs[i].S;
                    resObj.updTImeUTC = (int)(DateTime.UtcNow - utcHelper).TotalSeconds;

                    resObj.teams[0].name = objs[i].O1;
                    resObj.teams[1].name = objs[i].O2;
                }                
            }
            //

            File.WriteAllText("12.txt", serializer.Serialize(Program.games));
        }

    }
}

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace _1xParser
{
    static class Parser
    {
        private static DateTime utcHelper = new DateTime(1970, 1, 1);
        private static DateTime lastLNParseTime = DateTime.MinValue;
        private static DateTime lastLVParseTime = DateTime.MinValue;
        public static void ParseLine(long ID = 0)
        {
            if (lastLNParseTime.AddSeconds(15) > DateTime.Now)
                return;

            Utilites.cMsg("Parsing Line Page");
            string strRes = Parse("https://1xstavka.ru/line/Handball/");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            jsonFormats.LineRootObj[] objs;
            try
            {
                objs = serializer.Deserialize<jsonFormats.LineRootObj[]>(strRes);
            }
            catch(Exception e)
            {
                Utilites.wrException(e);
                File.WriteAllText("321.txt", strRes);
                return;
            }
            if (objs == null)
                return;
            
            for (int i = 0; i < objs.Length; i++)
            {
                long id = objs[i].I;
                Game resObj = Program.games.ContainsKey(id) ? Program.games[id] : new Game();

                resObj.league = objs[i].L;
                resObj.startTimeUTC = objs[i].S;
                resObj.updTimeUTC = (int)(DateTime.UtcNow - utcHelper).TotalSeconds;
                if (objs[i].E.Length < 9)
                    continue;
                resObj.totalF = objs[i].E[8].P;

                resObj.teams[0].name = objs[i].O1;
                resObj.teams[1].name = objs[i].O2;
                
                Program.games[id] = resObj;
                Task task = new Task();
                task.gameID = id;
                if(id == ID)
                {
                    task.timeUTC = resObj.startTimeUTC + 600; //10 min
                    task.func = Algorithms.FirstAlg;
                    tasksMgr.AddTask(task);

                    task.timeUTC = resObj.startTimeUTC + 300; //5 min
                    task.func = Algorithms.SecondAlg;
                    tasksMgr.AddTask(task);

                    /*
                    task.timeUTC = resObj.startTimeUTC + 1800; //30 min
                    task.func = Algorithms.ThirdAlg;
                    tasksMgr.AddTask(task);*/
                }
                else
                {
                    task.timeUTC = resObj.startTimeUTC - 60;
                    task.func = ParseLine;
                    tasksMgr.AddTask(task);
                }
            }
            //

            lastLNParseTime = DateTime.Now;
        }

        public static void ParseLive()
        {
            if (lastLVParseTime.AddSeconds(15) > DateTime.Now)
                return;

            Utilites.cMsg("Parsing Live Page");
            string strRes = Parse("https://1xstavka.ru/live/Handball/");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            jsonFormats.LiveRootObj[] objs;
            try
            {
                objs = serializer.Deserialize<jsonFormats.LiveRootObj[]>(strRes);
            }
            catch (Exception e)
            {
                Utilites.wrException(e);
                File.WriteAllText("321live.txt", strRes);
                return;
            }
            if (objs == null)
                return;

            for (int i = 0; i < objs.Length; i++)
            {
                long id = objs[i].I;
                if (!Program.games.ContainsKey(id))
                    continue;
                Game resObj = Program.games[id];

                resObj.league = objs[i].L;
                resObj.startTimeUTC = objs[i].S;
                resObj.updTimeUTC = (int)(DateTime.UtcNow - utcHelper).TotalSeconds;
                resObj.gameTime = objs[i].SC.TS;
                if (objs[i].E.Length < 9)
                    continue;
                resObj.totalL = objs[i].E[8].P;

                resObj.teams[0].name = objs[i].O1;
                resObj.teams[1].name = objs[i].O2;


                Program.games[id] = resObj;
            }
            //

            lastLVParseTime = DateTime.Now;
        }
        static string Parse(string url)
        {
            string mass = Utilites.GetHTML(url);

            try
            {
                Match match = Regex.Match(mass, @"<script[^>]*?>.*?SSR_DASHBOARD(.*?);.*?<\/script>");
                if (match.Groups.Count > 0)
                {
                    mass = match.Groups[1].Value; //значение скобки рег выр
                }

                match = Regex.Match(mass, @"Value.*?:(\[.*\])");
                if (match.Groups.Count > 0)
                {
                    mass = match.Groups[1].Value; //значение скобки рег выр
                }

                return mass;
            }
            catch (Exception e)
            {
                Utilites.wrException(e);
                return null;
            }
        }
    }
}

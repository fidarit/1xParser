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

            for (int i = 0; i < objs.Length; i++)
            {
                long id = objs[i].CI;
                Game resObj = Program.games.ContainsKey(id) ? Program.games[id] : new Game();

                resObj.league = objs[i].L;
                resObj.startTimeUTC = objs[i].S;
                resObj.updTimeUTC = (int)(DateTime.UtcNow - utcHelper).TotalSeconds;
                resObj.totalF = objs[i].E[8].P;

                resObj.teams[0].name = objs[i].O1;
                resObj.teams[1].name = objs[i].O2;


                Program.games[i] = resObj;
                Task task = new Task();
                task.gameID = id;
                if(id == ID)
                {
                    task.dt = resObj.startTimeUTC + 600; //10 min
                    task.func = Algorithms.FirstAlg;
                    tasksMgr.AddTask(task);

                    task.dt = resObj.startTimeUTC + 300; //5 min
                    task.func = Algorithms.SecondAlg;
                    tasksMgr.AddTask(task);

                    task.dt = resObj.startTimeUTC + 1800; //30 min
                    task.func = Algorithms.ThirdAlg;
                    tasksMgr.AddTask(task);
                }
                else
                {
                    task.dt = resObj.startTimeUTC - 60;
                    task.func = ParseLine;
                    tasksMgr.AddTask(task);
                }
            }
            //

            lastLNParseTime = DateTime.Now;
            File.WriteAllText("1.txt", serializer.Serialize(Program.games.Values));
        }

        public static void ParseLive()
        {
            if (lastLVParseTime.AddSeconds(15) > DateTime.Now)
                return;

            Utilites.cMsg("Parsing Live Page");

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

            for (int i = 0; i < objs.Length; i++)
            {
                long id = objs[i].CI;
                if (!Program.games.ContainsKey(id))
                    continue;
                Game resObj = Program.games[id];

                resObj.league = objs[i].L;
                resObj.startTimeUTC = objs[i].S;
                resObj.updTimeUTC = (int)(DateTime.UtcNow - utcHelper).TotalSeconds;
                resObj.totalL = objs[i].E[8].P;

                resObj.teams[0].name = objs[i].O1;
                resObj.teams[1].name = objs[i].O2;


                Program.games[i] = resObj;
            }
            //

            lastLVParseTime = DateTime.Now;
            File.WriteAllText("12.txt", serializer.Serialize(Program.games.Values));
        }

    }
}

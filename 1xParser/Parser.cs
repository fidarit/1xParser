using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;

namespace _1xParser
{
    static class Parser
    {
        private static DateTime lastLNParseTime = DateTime.MinValue;
        private static DateTime lastLVParseTime = DateTime.MinValue;
        public static void ParseLine(long ID = -1)
        {
            if (lastLNParseTime.AddSeconds(15) > DateTime.Now)
                Thread.Sleep(11000);

            Utilites.Log("Parsing Line Page");
            string strRes = Parse("https://1xstavka.ru/line/Handball/");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            jsonFormats.LineRootObj[] objs;
            try
            {
                objs = serializer.Deserialize<jsonFormats.LineRootObj[]>(strRes);
            }
            catch(Exception e)
            {
                Utilites.LogException(e);
                return;
            }
            if (objs == null)
                return;
            
            for (int i = 0; i < objs.Length; i++)
            {
                long id = objs[i].I;
                Game resObj;
                lock (Program.gamesLocker)
                {
                    resObj = Program.games.ContainsKey(id) ? Program.games[id] : new Game();
                }
                if (resObj.updTimeUNIX + 300 > Utilites.NowUNIX())
                    continue;

                resObj.league = objs[i].L;
                resObj.startTimeUNIX = objs[i].S;
                resObj.updTimeUNIX = Utilites.NowUNIX();
                if (objs[i].E.Length < 9)
                    continue;
                resObj.totalF = objs[i].E[8].P;

                resObj.teams[0].name = objs[i].O1;
                resObj.teams[1].name = objs[i].O2;

                lock (Program.gamesLocker)
                {
                    Program.games[id] = resObj;
                }
                if(id == ID)
                {
                    if (objs[i].E[0].C <= 1.6)
                        resObj.favTeam = 0;
                    else if(objs[i].E[2].C <= 1.6)
                        resObj.favTeam = 1;

                    Task task = new Task
                    {
                        GameID = id,
                        TimeUNIX = resObj.startTimeUNIX + 600, //10 min
                        Func = Algorithms.FirstAlg
                    };
                    TasksMgr.AddTask(task);

                    task = new Task
                    {
                        GameID = id,
                        TimeUNIX = resObj.startTimeUNIX + 300, //5 min
                        Func = Algorithms.SecondAlg
                    };
                    TasksMgr.AddTask(task);

                    if (resObj.favTeam >= 0)
                    {
                        task = new Task
                        {
                            GameID = id,
                            TimeUNIX = resObj.startTimeUNIX + 1800, //30 min
                            Func = Algorithms.ThirdAlg
                        };
                        TasksMgr.AddTask(task);
                    }
                }
                else
                {
                    int rand = (int)(new Random().NextDouble() * 200);
                    Task task = new Task
                    {
                        GameID = id,
                        TimeUNIX = resObj.startTimeUNIX - 300 + rand,
                        Func = ParseLine
                    };
                    TasksMgr.AddTask(task);
                }
            }
            //

            JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
            File.WriteAllText("games.txt", scriptSerializer.Serialize(Program.games.Values));


            lastLNParseTime = DateTime.Now;
        }

        public static void ParseLive()
        {
            if (lastLVParseTime.AddSeconds(15) > DateTime.Now)
                return;

            Utilites.Log("Parsing Live Page");
            string strRes = Parse("https://1xstavka.ru/live/Handball/");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            jsonFormats.LiveRootObj[] objs;
            try
            {
                objs = serializer.Deserialize<jsonFormats.LiveRootObj[]>(strRes);
            }
            catch (Exception e)
            {
                Utilites.LogException(e);
                return;
            }
            if (objs == null)
                return;

            for (int i = 0; i < objs.Length; i++)
            {
                long id = objs[i].I;
                Game resObj;
                lock (Program.gamesLocker)
                {
                    if (!Program.games.ContainsKey(id))
                        continue;
                    resObj = Program.games[id];
                }

                resObj.league = objs[i].L;
                resObj.startTimeUNIX = objs[i].S;
                resObj.updTimeUNIX = Utilites.NowUNIX();
                resObj.gameTime = objs[i].SC.TS;
                if (objs[i].E.Length < 9)
                    continue;
                resObj.totalL = objs[i].E[8].P;

                resObj.teams[0].name = objs[i].O1;
                resObj.teams[1].name = objs[i].O2;


                lock (Program.gamesLocker)
                {
                    Program.games[id] = resObj;
                }
            }
            //

            lastLVParseTime = DateTime.Now;
        }
        static string Parse(string url)
        {
            string mass = Utilites.GetHTML(url);

            try
            {
                Match match = Regex.Match(mass, '"' + "Value.*?:(\\[.*\\]).*SSR_TOP_SPORTS");
                if (match.Groups.Count > 0)
                {
                    mass = match.Groups[1].Value; //значение скобки рег выр
                }

                return mass;
            }
            catch (Exception e)
            {
                Utilites.LogException(e);
                return null;
            }
        }
    }
}

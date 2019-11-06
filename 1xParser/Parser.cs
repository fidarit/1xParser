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
            if (lastLNParseTime.AddSeconds(5) > DateTime.Now)
                Thread.Sleep(4000);

            Utilites.Log("Parsing Line Page");
            jsonFormats.ValueLN[] objs;
            try
            {
                string url = "https://1xstavka.ru/LineFeed/Get1x2_VZip?sports=8&count=50&mode=4&country=1&partner=51&getEmpty=true";
                string strRes = Utilites.GetHTML(url);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                jsonFormats.LineRootObj obj = serializer.Deserialize<jsonFormats.LineRootObj>(strRes);
                objs = obj.Value;
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
                jsonFormats.ValueLN obj = objs[i];
                long id = obj.N;
                Game resObj;

                obj.E = RebuidE_array(obj.E);
                if (obj.E == null)
                    continue;

                lock (Program.gamesLocker)
                {
                    resObj = Program.games.ContainsKey(id) ? Program.games[id] : new Game();

                    resObj.league = obj.L;
                    resObj.startTimeUNIX = obj.S;
                    resObj.updTimeUNIX = Utilites.NowUNIX();
                    if (obj.E.Length < 9)
                        continue;
                    resObj.totalF = obj.E[8].P;

                    resObj.teams[0].name = obj.O1;
                    resObj.teams[1].name = obj.O2;

                    Program.games[id] = resObj;
                }
                if(resObj.startTimeUNIX < Utilites.NowUNIX() + 301)
                {
                    if (obj.E[0].C > 0 && obj.E[0].C <= 1.6)
                        resObj.favTeam = 0;
                    else if(obj.E[2].C > 0 && obj.E[2].C <= 1.6)
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
                    int rand = (int)(new Random().NextDouble() * 150);
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
            if (lastLVParseTime.AddSeconds(5) > DateTime.Now)
                return;

            Utilites.Log("Parsing Live Page");
            jsonFormats.ValueLV[] objs;
            try
            {
                string url = "https://1xstavka.ru/LiveFeed/Get1x2_VZip?sports=8&count=50&mode=4&country=1&partner=51&getEmpty=true";
                string strRes = Utilites.GetHTML(url);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                jsonFormats.LiveRootObj obj = serializer.Deserialize<jsonFormats.LiveRootObj>(strRes);
                objs = obj.Value;
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
                jsonFormats.ValueLV obj = objs[i];
                long id = obj.N;
                Game resObj;

                obj.E = RebuidE_array(obj.E);
                if (obj.E == null)
                    continue;

                lock (Program.gamesLocker)
                {
                    if (!Program.games.ContainsKey(id))
                        continue;
                    resObj = Program.games[id];

                    resObj.league = obj.L;
                    resObj.startTimeUNIX = obj.S;
                    resObj.updTimeUNIX = Utilites.NowUNIX();
                    resObj.gameTime = obj.SC.TS;
                    if (obj.E.Length < 9)
                        continue;
                    resObj.totalL = obj.E[8].P;

                    resObj.teams[0].name = obj.O1;
                    resObj.teams[1].name = obj.O2;

                    Program.games[id] = resObj;
                }
            }
            //

            lastLVParseTime = DateTime.Now;
        }
        static jsonFormats.E[] RebuidE_array(jsonFormats.E[] e)
        {
            if (e == null || e.Length == 0)
                return null;

            try
            {
                int Tmax = 0;
                foreach (jsonFormats.E em in e)
                {
                    Tmax = Tmax < em.T ? em.T : Tmax;
                }

                jsonFormats.E[] es = new jsonFormats.E[Tmax];

                foreach (jsonFormats.E em in e)
                {
                    es[em.T - 1] = em;
                }
                return e;
            }
            catch(Exception ex)
            {
                Utilites.LogException(ex);
                return null;
            }
        }
    }
}

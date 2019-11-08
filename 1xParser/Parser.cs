using System;
using System.Web.Script.Serialization;

namespace _1xParser
{
    static class Parser
    {
        private static DateTime lastLNParseTime = DateTime.MinValue;
        private static DateTime lastLVParseTime = DateTime.MinValue;
        public static void ParseLine(int ID = -1)
        {
            if (lastLNParseTime.AddSeconds(5) > DateTime.Now)
                return;

            Utilites.Log("Проверяю страницу \"Линия\"");
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
                int id = obj.N;
                Game resObj;

                obj.E = RebuidE_array(obj.E);
                if (obj.E == null)
                    continue;

                bool containsGame;
                lock (Program.gamesLocker)
                {
                    containsGame = Program.games.ContainsKey(id);
                    resObj = containsGame ? Program.games[id] : new Game();

                    resObj.league = obj.L;
                    resObj.startTimeUNIX = obj.S;
                    resObj.updTimeUNIX = Utilites.NowUNIX();
                    if (obj.E.Length < 10)
                        continue;
                    resObj.totalF = obj.E[8].P;
                    resObj.TkfMore = obj.E[8].C;
                    resObj.TkfLess = obj.E[9].C;

                    resObj.teams[0].name = obj.O1;
                    resObj.teams[1].name = obj.O2;

                    resObj.teams[0].kf = obj.E[0].C;
                    resObj.teams[1].kf = obj.E[2].C;

                    Program.games[id] = resObj;
                }
                if (resObj.startTimeUNIX < Utilites.NowUNIX() + 301)
                {
                    if (resObj.teams[0].kf > 0 && resObj.teams[0].kf <= 1.6)
                        resObj.favTeam = 0;
                    else if (resObj.teams[1].kf > 0 && resObj.teams[1].kf <= 1.6)
                        resObj.favTeam = 1;

                    if (obj.E.Length > 13 && resObj.favTeam > -1)
                    {
                        resObj.iTotalF = obj.E[12 + resObj.favTeam].P;
                    }

                    Task task;
                    if (!resObj.algActived[0])
                    {
                        task = new Task
                        {
                            GameID = id,
                            TimeUNIX = resObj.startTimeUNIX + 600, //10 min
                            Func = Algorithms.FirstAlg
                        };
                        TasksMgr.AddTask(task);
                        resObj.algActived[0] = true;
                    }
                    if (!resObj.algActived[1])
                    {
                        task = new Task
                        {
                            GameID = id,
                            TimeUNIX = resObj.startTimeUNIX + 300, //5 min
                            Func = Algorithms.SecondAlg
                        };
                        TasksMgr.AddTask(task);
                        resObj.algActived[1] = true;
                    }

                    if (!resObj.algActived[2] && resObj.favTeam >= 0)
                    {
                        task = new Task
                        {
                            GameID = id,
                            TimeUNIX = resObj.startTimeUNIX + 1800, //30 min
                            Func = Algorithms.ThirdAlg
                        };
                        TasksMgr.AddTask(task);
                        resObj.algActived[2] = true;
                    }
                }
                else if(!containsGame)
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

            lastLNParseTime = DateTime.Now;
        }
        public static void ParseLive()
        {
            if (lastLVParseTime.AddSeconds(5) > DateTime.Now)
                return;

            Utilites.Log("Проверяю страницу \"Live\"");
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
                int id = obj.N;
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
                    if (obj.E.Length < 10)
                        continue;

                    resObj.totalL = obj.E[8].P;

                    if (obj.E.Length > 13 && resObj.favTeam > -1)
                    {
                        if (resObj.favTeam == 0)
                        {
                            resObj.iTotalL = obj.E[10].P;
                            resObj.iTkfMore = obj.E[10].C;
                            resObj.iTkfLess = obj.E[11].C;
                        }
                        else
                        {
                            resObj.iTotalL = obj.E[12].P;
                            resObj.iTkfMore = obj.E[12].C;
                            resObj.iTkfLess = obj.E[13].C;
                        }
                    }

                    if(obj.SC != null && obj.SC.PS != null && obj.SC.PS.Length > 0)
                    {
                        resObj.teams[0].goals1T = obj.SC.PS[0].Value.S1;
                        resObj.teams[1].goals1T = obj.SC.PS[0].Value.S2;
                    }

                    resObj.TkfMore = obj.E[8].C;
                    resObj.TkfLess = obj.E[9].C;

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
                for(int i = 0; i < Tmax; i++)
                {
                    es[i] = new jsonFormats.E();
                }

                foreach (jsonFormats.E em in e)
                {
                    es[em.T - 1] = em;
                }
                return es;
            }
            catch(Exception ex)
            {
                Utilites.LogException(ex);
                return null;
            }
        }
    }
}

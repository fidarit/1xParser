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
            jsonFormats.ValueLN[] results;
            try
            {
                string url = "https://1xstavka.ru/LineFeed/Get1x2_VZip?sports=8&count=50&mode=4&country=1&partner=51&getEmpty=true";
                string strRes = Utilites.GetHTML(url);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                jsonFormats.LineRootObj obj = serializer.Deserialize<jsonFormats.LineRootObj>(strRes);
                results = obj.Value;
            }
            catch(Exception e)
            {
                Utilites.LogException(e);
                return;
            }
            if (results == null)
                return;
            
            for (int i = 0; i < results.Length; i++)
            {
                jsonFormats.ValueLN result = results[i];
                int id = result.N;
                Game game;

                result.E = RebuidE_array(result.E);
                if (result.E == null)
                    continue;

                bool containsGame;
                lock (Program.gamesLocker)
                {
                    containsGame = Program.games.ContainsKey(id);
                    game = containsGame ? Program.games[id] : new Game();

                    game.league = result.L;
                    game.startTimeUNIX = result.S;
                    game.updTimeUNIX = Utilites.NowUNIX();
                    if (result.E.Length < 10)
                        continue;
                    game.totalF = result.E[8].P;
                    game.TkfMore = result.E[8].C;
                    game.TkfLess = result.E[9].C;

                    game.teams[0].name = result.O1;
                    game.teams[1].name = result.O2;

                    game.teams[0].kf = result.E[0].C;
                    game.teams[1].kf = result.E[2].C;

                    Program.games[id] = game;
                }
                if (game.startTimeUNIX < Utilites.NowUNIX() + 301)
                {
                    if (game.teams[0].kf > 0 && game.teams[0].kf <= 1.6)
                        game.favTeam = 0;
                    else if (game.teams[1].kf > 0 && game.teams[1].kf <= 1.6)
                        game.favTeam = 1;

                    if (result.E.Length > 13 && game.favTeam > -1)
                    {
                        game.iTotalF = result.E[12 + game.favTeam].P;
                    }

                    Task task;
                    if (!game.deleteFuncIsActivated) //Алгоритм, который проверяет и удаляет игру в конце
                    {
                        task = new Task
                        {
                            GameID = id,
                            TimeUNIX = game.startTimeUNIX + 3660, //61 min
                            Func = Algorithms.CheckOnTheEnd
                        };
                        TasksMgr.AddTask(task);
                        game.deleteFuncIsActivated = true;
                    }
                    if (!game.algoritms[0].actived)
                    {
                        task = new Task
                        {
                            GameID = id,
                            TimeUNIX = game.startTimeUNIX + 600, //10 min
                            Func = Algorithms.FirstAlg
                        };
                        TasksMgr.AddTask(task);
                        game.algoritms[0].actived = true;
                    }
                    if (!game.algoritms[1].actived)
                    {
                        task = new Task
                        {
                            GameID = id,
                            TimeUNIX = game.startTimeUNIX + 300, //5 min
                            Func = Algorithms.SecondAlg
                        };
                        TasksMgr.AddTask(task);
                        game.algoritms[1].actived = true;
                    }
                    if (!game.algoritms[2].actived && game.favTeam >= 0)
                    {
                        task = new Task
                        {
                            GameID = id,
                            TimeUNIX = game.startTimeUNIX + 1800, //30 min
                            Func = Algorithms.ThirdAlg
                        };
                        TasksMgr.AddTask(task);
                        game.algoritms[2].actived = true;
                    }
                }
                else if(!containsGame)
                {
                    int rand = (int)(new Random().NextDouble() * 150);
                    Task task = new Task
                    {
                        GameID = id,
                        TimeUNIX = game.startTimeUNIX - 300 + rand,
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
            jsonFormats.ValueLV[] results;
            try
            {
                string url = "https://1xstavka.ru/LiveFeed/Get1x2_VZip?sports=8&count=50&mode=4&country=1&partner=51&getEmpty=true";
                string strRes = Utilites.GetHTML(url);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                jsonFormats.LiveRootObj obj = serializer.Deserialize<jsonFormats.LiveRootObj>(strRes);
                results = obj.Value;
            }
            catch (Exception e)
            {
                Utilites.LogException(e);
                return;
            }
            if (results == null)
                return;

            for (int i = 0; i < results.Length; i++)
            {
                jsonFormats.ValueLV result = results[i];
                int id = result.N;
                Game game;

                result.E = RebuidE_array(result.E);
                if (result.E == null)
                    continue;

                lock (Program.gamesLocker)
                {
                    if (!Program.games.ContainsKey(id))
                        continue;
                    game = Program.games[id];

                    game.startTimeUNIX = result.S;
                    game.updTimeUNIX = Utilites.NowUNIX();
                    game.gameTime = result.SC.TS;
                    if (result.E.Length < 10)
                        continue;

                    game.totalL = result.E[8].P;

                    if (result.E.Length > 13 && game.favTeam > -1)
                    {
                        if (game.favTeam == 0)
                        {
                            game.iTotalL = result.E[10].P;
                            game.iTkfMore = result.E[10].C;
                            game.iTkfLess = result.E[11].C;
                        }
                        else
                        {
                            game.iTotalL = result.E[12].P;
                            game.iTkfMore = result.E[12].C;
                            game.iTkfLess = result.E[13].C;
                        }
                    }

                    if(result.SC != null && result.SC.PS != null && result.SC.PS.Length > 0)
                    {
                        game.teams[0].goals1T = result.SC.PS[0].Value.S1;
                        game.teams[1].goals1T = result.SC.PS[0].Value.S2;
                    }

                    game.TkfMore = result.E[8].C;
                    game.TkfLess = result.E[9].C;

                    game.teams[0].name = result.O1;
                    game.teams[1].name = result.O2;

                    Program.games[id] = game;
                }
            }
            //

            lastLVParseTime = DateTime.Now;
        }
        public static void ParseEndGameResults(int id)
        {
            jsonFormats.ValueGR result;
            try
            {
                string url = "https://1xstavka.ru/LiveFeed/GetGameZip?id=" + id + "&lng=ru&partner=51";
                string strRes = Utilites.GetHTML(url);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                jsonFormats.GameResRootObj obj = serializer.Deserialize<jsonFormats.GameResRootObj>(strRes);
                result = obj.Value;
            }
            catch (Exception e)
            {
                Utilites.LogException(e);
                return;
            }

            //if game wasn't finish
            if (!result.F)
                return;

            lock (Program.gamesLocker)
            {
                if (!Program.games.ContainsKey(id))
                    return;
                Game game = Program.games[id];
                game.isFinished = result.F;

                game.updTimeUNIX = Utilites.NowUNIX();
                game.gameTime = result.SC.TS;

                if (result.SC != null && result.SC.PS != null && result.SC.PS.Length > 0)
                {
                    game.teams[0].goals1T = result.SC.PS[0].Value.S1;
                    game.teams[1].goals1T = result.SC.PS[0].Value.S2;

                    game.teams[0].allGoals = result.SC.PS[0].Value.S1 + result.SC.PS[1].Value.S1;
                    game.teams[1].allGoals = result.SC.PS[0].Value.S2 + result.SC.PS[1].Value.S2;
                }

                Program.games[id] = game;
            }
        }
        static jsonFormats.E[] RebuidE_array(jsonFormats.E[] eArg)
        {
            if (eArg == null || eArg.Length == 0)
                return null;

            try
            {
                int Tmax = 0;
                foreach (jsonFormats.E em in eArg)
                {
                    Tmax = Tmax < em.T ? em.T : Tmax;
                }

                jsonFormats.E[] eResult = new jsonFormats.E[Tmax];
                for(int i = 0; i < Tmax; i++)
                {
                    eResult[i] = new jsonFormats.E();
                }

                foreach (jsonFormats.E eElem in eArg)
                {
                    eResult[eElem.T - 1] = eElem;
                }
                return eResult;
            }
            catch(Exception ex)
            {
                Utilites.LogException(ex);
                return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _1xParser
{
    static class Algorithms
    {
        public static void FirstAlg(long id)
        {
            Parser.ParseLive();
            Game game;
            lock (Program.gamesLocker)
            {
                game = Program.games[id];
            }

            if (game.gameTime > 2700) //45 min
                return;

            int ret = 1;
            if (game.updTimeUNIX + 30 > Utilites.NowUNIX() && game.totalF * game.totalL > 0)
            {
                string team1 = game.teams[0].name;
                string team2 = game.teams[1].name;
                if(game.teams[0].kf * game.teams[1].kf > 0)
                {
                    team1 += " (" + game.teams[0].kf + ")";
                    team2 += " (" + game.teams[1].kf + ")";
                }

                double deltaTotal = game.totalL - game.totalF;
                string rec = "";
                if (deltaTotal >= 8)
                    rec = "ТМ " + game.totalL + " - " + game.TkfLess;
                else if (deltaTotal <= -7)
                    rec = "ТБ " + game.totalL + " - " + game.TkfMore;

                if (rec.Length > 0)
                {
                    Telegram.SendMessageToAll("Алгоритм - \"Тотал Матча\""
                        + "\nЛига - \"" + game.league + "\""
                        + "\nКоманда - \"" + team1 + " - " + team2 + "\""
                        + "\nВремя - \"" + TimeSpan.FromSeconds(game.gameTime).ToString("mm\\:ss") + "\""
                        + "\n"
                        + "\nНачальный тотал -  \"" + game.totalF + "\""
                        + "\n"
                        + "\nСейчас тотал -  \"" + game.totalL + "\""
                        + "\nРазница тотала - \"" + deltaTotal + "\""
                        + "\nРекомендую - \"" + rec + "\"");
                }
            }
            if (ret != 0)
            {
                TasksMgr.AddTask(new Task
                {
                    TimeUNIX = Utilites.NowUNIX() + 25,
                    Func = FirstAlg,
                    GameID = id
                });
            }
        }
        public static void SecondAlg(long id)
        {
            Parser.ParseLive();
            Game game;
            lock (Program.gamesLocker)
            {
                game = Program.games[id];
            }

            if (game.gameTime > 1200) //20 min
                return;

            int ret = 1;
            if (game.updTimeUNIX + 30 > Utilites.NowUNIX() && game.totalF * game.totalL > 0)
            {
                string team1 = game.teams[0].name;
                string team2 = game.teams[1].name;
                if (game.teams[0].kf * game.teams[1].kf > 0)
                {
                    team1 += " (" + game.teams[0].kf + ")";
                    team2 += " (" + game.teams[1].kf + ")";
                }

                double totalF = Math.Round(game.totalF / 2);
                double totalL = Math.Round(game.totalL / 2);
                double deltaTotal = totalF - totalL;
                string rec = "";
                if (deltaTotal >= 6)
                    rec = "ТМ " + totalL + " - " + game.TkfLess;
                else if (deltaTotal <= -5)
                    rec = "ТБ " + totalL + " - " + game.TkfLess;

                if (rec.Length > 0)
                {
                    ret = Telegram.SendMessageToAll("Алгоритм - \"Тотал в 1 тайме\""
                        + "\nЛига - \"" + game.league + "\""
                        + "\nКоманда - \"" + team1 + " - " + team2 + "\""
                        + "\nВремя - \"" + TimeSpan.FromSeconds(game.gameTime).ToString("mm\\:ss") + "\""
                        + "\n"
                        + "\nНачальный тотал -  \"" + totalF + "\""
                        + "\n"
                        + "\nСейчас тотал -  \"" + totalL + "\""
                        + "\nРазница тотала - \"" + deltaTotal + "\""
                        + "\nРекомендую - \"" + rec + "\"");
                }
            }
            if (ret != 0)
            {
                TasksMgr.AddTask(new Task
                {
                    TimeUNIX = Utilites.NowUNIX() + 25,
                    Func = SecondAlg,
                    GameID = id
                });
            }
        }
        public static void ThirdAlg(long id)
        {
            Parser.ParseLive();
            Game game;
            lock (Program.gamesLocker)
            {
                game = Program.games[id];
            }

            if (game.gameTime >= 1800 && game.gameTime < 1810 && game.updTimeUNIX + 10 < Utilites.NowUNIX())
            {
                TasksMgr.AddTask(new Task
                {
                    TimeUNIX = Utilites.NowUNIX() + 5,
                    Func = ThirdAlg,
                    GameID = id
                });
                return;
            }

            if (game.favTeam > -1 && game.iTotalF * game.iTotalL > 0)
            {
                string team1 = game.teams[0].name;
                string team2 = game.teams[1].name;
                if (game.teams[0].kf * game.teams[1].kf > 0)
                {
                    team1 += " (" + game.teams[0].kf + ")";
                    team2 += " (" + game.teams[1].kf + ")";
                }

                double totalMF = Math.Round(game.iTotalF / 2);
                double totalML = Math.Round(game.iTotalL / 2);

                double favGoalsAim = totalMF - 4;
                double realGoals = game.teams[game.favTeam].goals1T;

                double totalL = game.iTotalL;

                string favTeam = game.teams[game.favTeam].name;
                string rec = "ТБ " + game.iTotalL + " - " + game.iTkfLess;

                if (realGoals <= favGoalsAim && totalL < totalMF + realGoals)
                {
                    Telegram.SendMessageToAll("Алгоритм - \"Индивидуальный тотал Фаворита\""
                        + "\nЛига - \"" + game.league + "\""
                        + "\nКоманда - \"" + team1 + " - " + team2 + "\""
                        + "\nВремя - \"" + TimeSpan.FromSeconds(game.gameTime).ToString("mm\\:ss") + "\""
                        + "\n"
                        + "\nНачальный тотал -  \"" + totalMF + "\""
                        + "\n"
                        + "\nСейчас тотал -  \"" + totalML + "\"" 
                        + "\nФаворит - \"" + favTeam + "\""
                        + "\nРекомендую - \"" + rec + "\""); 
                }
            }
        }
    }
}

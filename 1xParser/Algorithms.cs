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

            if (game.updTimeUNIX + 30 < Utilites.NowUNIX() || game.gameTime > 2700) //45 min
                return;

            TasksMgr.AddTask(new Task
            {
                TimeUNIX = Utilites.NowUNIX() + 30,
                Func = FirstAlg,
                GameID = id
            });

            double deltaTotal = game.totalL - game.totalF;
            string rec = "";
            if (deltaTotal >= 8)
                rec = "ТМ";
            else if (deltaTotal <= 7)
                rec = "ТБ";
            else
                return;

            Telegram.SendMessageToAll("Алгоритм - \"Тотал Матча\""
                + "<br />Лига - \"" + game.league + "\""
                + "<br />Команда - \"" + game.teams[0].name + " - " + game.teams[1].name + "\""
                + "<br />Время - \"" + TimeSpan.FromSeconds(game.gameTime).ToString("mm\\:ss") + "\""
                + "<br />Начальный тотал -  \"" + game.totalF + "\""
                + "<br />Сейчас тотал -  \"" + game.totalL + "\""
                + "<br />Разница тотала - \"" + deltaTotal + "\""
                + "<br />Рекомендую - \"" + rec + "\"");
        }
        public static void SecondAlg(long id)
        {
            Parser.ParseLive();
            Game game;
            lock (Program.gamesLocker)
            {
                game = Program.games[id];
            }

            if (game.updTimeUNIX + 30 < Utilites.NowUNIX() || game.gameTime > 1200) //20 min
                return;
            
            TasksMgr.AddTask(new Task
            {
                TimeUNIX = Utilites.NowUNIX() + 30,
                Func = SecondAlg,
                GameID = id
            });

            double totalF = game.totalF / 2;
            double totalL = game.totalL / 2;
            double deltaTotal = totalF - totalL;
            string rec = "";
            if (deltaTotal >= 6)
                rec = "ТМ";
            else if (deltaTotal <= 5)
                rec = "ТБ";
            else
                return;

            Telegram.SendMessageToAll("Алгоритм - \"Тотал в 1 тайме\""
                + "<br />Лига - \"" + game.league + "\""
                + "<br />Команда - \"" + game.teams[0].name + " - " + game.teams[1].name + "\""
                + "<br />Время - \"" + TimeSpan.FromSeconds(game.gameTime).ToString("mm\\:ss") + "\""
                + "<br />Начальный тотал -  \"" + game.totalF + "\""
                + "<br />Сейчас тотал -  \"" + game.totalL + "\""
                + "<br />Разница тотала - \"" + deltaTotal + "\""
                + "<br />Рекомендую - \"" + rec + "\"");
        }
        public static void ThirdAlg(long id)
        {
            Parser.ParseLive();
            Game game;
            lock (Program.gamesLocker)
            {
                game = Program.games[id];
            }

            TasksMgr.AddTask(new Task
            {
                TimeUNIX = Utilites.NowUNIX() + 30,
                Func = ThirdAlg,
                GameID = id
            });
        }
    }
}

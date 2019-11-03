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
            Game game = Program.games[id];
            int gameTime = (int)DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds - game.startTimeUTC;

            if (gameTime > 2700) //45 min
                return;

            tasksMgr.AddTask(new Task
            {
                timeUTC = (int)DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds + 30,
                func = FirstAlg,
                gameID = id
            });

            double deltaTotal = game.totalF - game.totalL;
            string rec = "";
            if (deltaTotal >= 8)
                rec = "ТБ";
            else if (deltaTotal <= 7)
                rec = "ТМ";
            else
                return;

            Telegram.SendMessageToAll("Алгоритм - \"Тотал Матча\""
                + "<br />Лига - \"" + game.league + "\""
                + "<br />Команда - \"" + 2 + "\""
                + "<br />Время - \"" + TimeSpan.FromSeconds(gameTime).ToString("") + "\""
                + "<br />Начальный тотал -  \"" + game.totalF + "\""
                + "<br />Сейчас тотал -  \"" + game.totalL + "\""
                + "<br />Разница тотала - \"" + deltaTotal + "\""
                + "<br />Рекомендую - \"" + rec + "\"");
        }
        public static void SecondAlg(long id)
        {
            Parser.ParseLive();

            tasksMgr.AddTask(new Task
            {
                timeUTC = (int)DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds + 30,
                func = SecondAlg,
                gameID = id
            });
        }
        public static void ThirdAlg(long id)
        {
            Parser.ParseLive();

            tasksMgr.AddTask(new Task
            {
                timeUTC = (int)DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds + 30,
                func = ThirdAlg,
                gameID = id
            });
        }
    }
}

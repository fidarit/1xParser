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

            Telegram.SendMessageToAll("	Алгоритм - \"Тотал Матча\""
                + "Лига - \"" + game.league + "\""
                + "Команда - \"" + 2 + "\""
                + "Время - \"" + TimeSpan.FromSeconds(gameTime).ToString("") + "\""
                + "Начальный тотал -  \"" + game.totalF + "\""
                + "Сейчас тотал -  \"" + game.totalL + "\""
                + "Разница тотала - \"" + (game.totalF - game.totalL).ToString() + "\""
                + "Рекомендую - \"ТБ или ТМ\"");

            tasksMgr.AddTask(new Task
            {
                dt = (int)DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds + 30,
                func = FirstAlg,
                gameID = id
            });
        }
        public static void SecondAlg(long id)
        {
            Parser.ParseLive();

            tasksMgr.AddTask(new Task
            {
                dt = (int)DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds + 30,
                func = SecondAlg,
                gameID = id
            });
        }
        public static void ThirdAlg(long id)
        {
            Parser.ParseLive();

            tasksMgr.AddTask(new Task
            {
                dt = (int)DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds + 30,
                func = ThirdAlg,
                gameID = id
            });
        }
    }
}

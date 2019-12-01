using System;
using System.Text;

namespace _1xParser
{
    static class Algorithms
    {
        public static void FirstAlg(int id)
        {
            Parser.ParseLive();
            lock (Program.gamesLocker)
            {
                Game game = Program.games[id];

                if (game.gameTime > 2700) //45 min
                    return;

                bool ret = false;
                if (game.updTimeUNIX + 30 > Utilites.NowUNIX() && game.totalF * game.totalL > 0)
                {
                    string team1 = game.teams[0].name;
                    string team2 = game.teams[1].name;

                    Utilites.Log("Проверка игры \"" + team1 + " - " + team2 + "\" на \"Тотал Матча\"");

                    if (game.teams[0].kf * game.teams[1].kf > 0)
                    {
                        team1 += " (" + game.teams[0].kf + ")";
                        team2 += " (" + game.teams[1].kf + ")";
                    }

                    double deltaTotal = game.totalL - game.totalF;
                    string recomend = "";
                    if (deltaTotal >= 7)
                    {
                        recomend = "ТМ " + game.totalL + " - " + game.TkfLess;
                        Program.games[id].algoritms[0].sendedTotal = game.totalL;
                        Program.games[id].algoritms[0].tMore = false;
                    }
                    else if (deltaTotal <= -6)
                    {
                        recomend = "ТБ " + game.totalL + " - " + game.TkfMore;
                        Program.games[id].algoritms[0].sendedTotal = game.totalL;
                        Program.games[id].algoritms[0].tMore = false;
                    }

                    //Calculating rate
                    //
                    double rate = game.teams[0].goals1T + game.teams[1].goals1T;
                    //оставить 2 знака после запятой ииии 1800=60*60 из уравнения клиента
                    rate -= Math.Round(game.totalF * game.gameTime / 3600, 2);

                    string text = "#Signal_" + Params.LastSignalNumer
                            + "\nАлгоритм - \"Тотал Матча\""
                            + "\nЛига - \"" + game.league + "\""
                            + "\nКоманда - \"" + team1 + " - " + team2 + "\""
                            + "\nВремя - \"" + TimeSpan.FromSeconds(game.gameTime).ToString("mm\\:ss") + "\""
                            + "\nСчёт - \"" + game.teams[0].goals1T + ":" + game.teams[1].goals1T + "\""
                            + "\n"
                            + "\nНачальный тотал -  \"" + game.totalF + "\""
                            + "\n"
                            + "\nТемп -  \"" + rate + "\""
                            + "\nСейчас тотал -  \"" + game.totalL + "\""
                            + "\nРазница тотала - \"" + deltaTotal + "\""
                            + "\nРекомендую - \"" + recomend + "\"";

                    if (recomend.Length > 0)
                    {
                        ret = Telegram.SendMessagesFromAlgs(text, 1, id);
                    }
                }
                if (!ret)
                {
                    TasksMgr.AddTask(new Task
                    {
                        TimeUNIX = Utilites.NowUNIX() + 25,
                        Func = FirstAlg,
                        GameID = id
                    });
                }
                else //if signal was succesful sended to anybody
                    Params.LastSignalNumer++;
            }
        }
        public static void SecondAlg(int id)
        {
            Parser.ParseLive();
            lock (Program.gamesLocker)
            {
                Game game = Program.games[id];

                if (game.gameTime > 1200) //20 min
                    return;

                bool ret = false;
                if (game.updTimeUNIX + 30 > Utilites.NowUNIX() && game.totalF * game.totalL > 0)
                {
                    string team1 = game.teams[0].name;
                    string team2 = game.teams[1].name;

                    Utilites.Log("Проверка игры \"" + team1 + " - " + team2 + "\" на \"Тотал в 1 тайме\"");

                    if (game.teams[0].kf * game.teams[1].kf > 0)
                    {
                        team1 += " (" + game.teams[0].kf + ")";
                        team2 += " (" + game.teams[1].kf + ")";
                    }

                    double totalF = Math.Round(game.totalF / 2);
                    double totalL = Math.Round(game.totalL / 2);
                    double deltaTotal = totalL - totalF;
                    string recomend = "";
                    if (deltaTotal >= 5)
                    {
                        recomend = "ТМ " + totalL + " - " + game.TkfLess;
                        Program.games[id].algoritms[1].sendedTotal = totalL;
                        Program.games[id].algoritms[1].tMore = false;
                    }
                    else if (deltaTotal <= -4)
                    {
                        recomend = "ТБ " + totalL + " - " + game.TkfMore;
                        Program.games[id].algoritms[1].sendedTotal = totalL;
                        Program.games[id].algoritms[1].tMore = true;
                    }

                    //Calculating rate
                    //
                    double rate = game.teams[0].goals1T + game.teams[1].goals1T;
                    //оставить 2 знака после запятой ииии 1800=30*60 из уравнения клиента
                    rate -= Math.Round(game.totalF * game.gameTime / 1800, 2); 

                    string text = "#Signal_" + Params.LastSignalNumer
                        + "\nАлгоритм - \"Тотал в 1 тайме\""
                        + "\nЛига - \"" + game.league + "\""
                        + "\nКоманда - \"" + team1 + " - " + team2 + "\""
                        + "\nВремя - \"" + TimeSpan.FromSeconds(game.gameTime).ToString("mm\\:ss") + "\""
                        + "\nСчёт - \"" + game.teams[0].goals1T + ":" + game.teams[1].goals1T + "\""
                        + "\n"
                        + "\nНачальный тотал -  \"" + totalF + "\""
                        + "\n"
                        + "\nТемп -  \"" + rate + "\""
                        + "\nСейчас тотал -  \"" + totalL + "\""
                        + "\nРазница тотала - \"" + deltaTotal + "\""
                        + "\nРекомендую - \"" + recomend + "\"";

                    if (recomend.Length > 0)
                    {
                        ret = Telegram.SendMessagesFromAlgs(text, 2, id);
                    }
                }
                if (!ret)
                {
                    TasksMgr.AddTask(new Task
                    {
                        TimeUNIX = Utilites.NowUNIX() + 25,
                        Func = SecondAlg,
                        GameID = id
                    });
                }
                else //if signal was succesful sended to anybody
                    Params.LastSignalNumer++;
            }
        }
        public static void ThirdAlg(int id)
        {
            Parser.ParseLive();
            lock (Program.gamesLocker)
            {
                Game game = Program.games[id];
                if (game.gameTime < 1800)
                {
                    TasksMgr.AddTask(new Task
                    {
                        TimeUNIX = Utilites.NowUNIX() + 1800 - game.gameTime,
                        Func = ThirdAlg,
                        GameID = id
                    });
                    return;
                }
                if (game.updTimeUNIX + 10 < Utilites.NowUNIX())
                {
                    TasksMgr.AddTask(new Task
                    {
                        TimeUNIX = Utilites.NowUNIX() + 5,
                        Func = ThirdAlg,
                        GameID = id
                    });
                    return;
                }

                if (game.gameTime > 1810 && game.favTeam > -1 && game.iTotalF * game.iTotalL > 0)
                {
                    string team1 = game.teams[0].name;
                    string team2 = game.teams[1].name;

                    Utilites.Log("Проверка игры \"" + team1 + " - " + team2 + "\" на \"Индивидуальный тотал Фаворита\"");

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
                    string recomend = "ТБ " + game.iTotalL + " - " + game.iTkfLess;

                    Program.games[id].algoritms[2].sendedTotal = game.iTotalL;
                    Program.games[id].algoritms[2].tMore = true;

                    string text = "#Signal_" + Params.LastSignalNumer
                        + "\nАлгоритм - \"Индивидуальный тотал Фаворита\""
                        + "\nЛига - \"" + game.league + "\""
                        + "\nКоманда - \"" + team1 + " - " + team2 + "\""
                        + "\nВремя - \"" + TimeSpan.FromSeconds(game.gameTime).ToString("mm\\:ss") + "\""
                        + "\nСчёт - \"" + game.teams[0].goals1T + ":" + game.teams[1].goals1T + "\""
                        + "\n"
                        + "\nНачальный тотал -  \"" + totalMF + "\""
                        + "\n"
                        + "\nСейчас тотал -  \"" + totalML + "\""
                        + "\nФаворит - \"" + favTeam + "\""
                        + "\nРекомендую - \"" + recomend + "\"";
                    if (realGoals <= favGoalsAim && totalL < totalMF + realGoals)
                    {
                        if(Telegram.SendMessagesFromAlgs(text, 3, id))
                            Params.LastSignalNumer++;
                    }
                }
            }
        }
        public static void CheckOnTheEnd(int id)
        {
            Parser.ParseEndGameResults(id);

            lock (Program.gamesLocker)
            {
                Game game = Program.games[id];

                //if game wasn't finished
                if (!game.isFinished)
                {
                    TasksMgr.AddTask(new Task
                    {
                        TimeUNIX = Utilites.NowUNIX() + 3660 - game.gameTime,
                        Func = CheckOnTheEnd,
                        GameID = id
                    });
                    return;
                }
                //
                string yes = Encoding.UTF8.GetString(new byte[] { 0xE2, 0x9C, 0x85 });
                string not = Encoding.UTF8.GetString(new byte[] { 0xE2, 0x9D, 0x8E });

                string text;
                double goals;
                Algoritm algoritm;
                //First algoritm
                algoritm = game.algoritms[0];
                if (algoritm.sendedTotal > 0)
                {
                    text = algoritm.messageText + " ";
                    goals = game.teams[0].allGoals + game.teams[1].allGoals;

                    if ((algoritm.tMore && goals > algoritm.sendedTotal)
                        || (!algoritm.tMore && goals < algoritm.sendedTotal))
                        text += yes;
                    else
                        text += not;

                    text += " " + goals;
                    
                    Utilites.Log("Редактирую сообщения");

                    foreach (Message message in algoritm.messages)
                    {
                        Telegram.EditMessage(text, message.chatID, message.msgID);
                    }
                }

                //Second algoritm
                algoritm = game.algoritms[1];
                if (algoritm.sendedTotal > 0)
                {
                    text = algoritm.messageText + " ";
                    goals = game.teams[0].goals1T + game.teams[1].goals1T;

                    if ((algoritm.tMore && goals > algoritm.sendedTotal)
                        || (!algoritm.tMore && goals < algoritm.sendedTotal))
                    {
                        text += yes;
                    }
                    else
                        text += not;

                    text += " " + goals;

                    foreach (Message message in algoritm.messages)
                    {
                        Telegram.EditMessage(text, message.chatID, message.msgID);
                    }
                }

                //Third algoritm
                algoritm = game.algoritms[2];
                if (algoritm.sendedTotal > 0)
                {
                    text = algoritm.messageText + " ";
                    goals = game.teams[game.favTeam - 1].goals1T;

                    if (goals > algoritm.sendedTotal)
                        text += yes;
                    else
                        text += not;

                    text += " " + goals;

                    foreach (Message message in algoritm.messages)
                    {
                        Telegram.EditMessage(text, message.chatID, message.msgID);
                    }
                }


                Program.games.Remove(id);
            }
        }
    }
}

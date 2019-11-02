﻿using System;
using System.Threading;
using System.Web.Script.Serialization;

namespace _1xParser
{
    static class Telegram
    {
        public static Thread msgUpdThread;

        public static int SendMessage(string text, int targetID)
        {
            Utilites.GET("https://api.telegram.org/bot" + Params.telegToken + "/sendMessage" +
                "?chat_id=" + targetID + "&text=" + text + "&parse_mode=HTML");

            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            // objs = serializer.Deserialize<RootObj[]>(mass);
            //Utilites.cMsg(ret);
            return 0;
        }
        public static int SendMessageToAll(string text)
        {
            for (int i = 0; i < Params.users.Count; i++)
            {
                Utilites.GET("https://api.telegram.org/bot" + Params.telegToken + "/sendMessage" +
                    "?chat_id=" + Params.users[i] + "&text=" + text + "&parse_mode=HTML");
            }

            //Utilites.cMsg(ret);
            return 0;
        }
        public static void StartMsgUpd()
        {
            msgUpdThread = new Thread(msgUpd);
            msgUpdThread.Start();
        }
        static void msgUpd()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            while (tasksMgr.doOtherThreads)
            {
                try
                {
                    string ret = Utilites.GET("https://api.telegram.org/bot" + Params.telegToken + "/getUpdates");
                    jsonFormats.GetUpdResRoot obj;

                    obj = serializer.Deserialize<jsonFormats.GetUpdResRoot>(ret);
                    if (obj != null && obj.ok && obj.result.Length > 0)
                    {
                        for (int i = 0; i < obj.result.Length; i++)
                        {
                            int id = obj.result[i].message.from.id;
                            if (obj.result[i].message.message_id > Params.lastUMid)
                            {
                                switch (obj.result[i].message.text)
                                {
                                    case "/start":
                                        if (Params.users.Contains(id))
                                        {
                                            SendMessage("Для вас уже включена рассылка", id);
                                        }
                                        else
                                        {
                                            Params.users.Add(id);
                                            SendMessage("Теперь вы будете получать рассылку", id);
                                        }
                                        break;
                                    case "/stop":
                                        if (Params.users.Contains(id))
                                        {
                                            Params.users.Remove(id);
                                            SendMessage("Теперь вы не будете получать рассылку", id);
                                        }
                                        else
                                        {
                                            SendMessage("Вы и так не получаете рассылку...", id);
                                        }
                                        break;
                                    default:
                                        SendMessageToAll("Извините, но пока я вас не понимаю...");
                                        Utilites.cWarning(obj.result[i].message.from.first_name + " пишет: " + obj.result[i].message.text);
                                        break;
                                }
                            }
                        }
                        Params.lastUMid = obj.result[obj.result.Length - 1].message.message_id;
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Utilites.cError(e.Message);
                }
            }
        }
    }
}

using System;
using System.Threading;
using System.Web.Script.Serialization;

namespace _1xParser
{
    static class Telegram
    {
        public static Thread msgUpdThread;

        public static int SendMessage(string text, int targetID)
        {
            string ret = Utilites.GET("https://api.telegram.org/bot" + Params.telegToken + "/sendMessage" +
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
                string ret = Utilites.GET("https://api.telegram.org/bot" + Params.telegToken + "/sendMessage" +
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
                            if(obj.result[i].message.message_id > Params.lastUMid)
                                switch (obj.result[i].message.text)
                                {
                                    case "/start":
                                        if (!Params.users.Contains(obj.result[i].message.from.id))
                                            Params.users.Add(obj.result[i].message.from.id);
                                        break;
                                    case "/stop":
                                        Params.users.Remove(obj.result[i].message.from.id);
                                        break;
                                }

                                
                                else if (obj.result[i].message.text == "/stop"
                                    && Params.users.Contains(obj.result[i].message.from.id))
                                    Params.users.Remove(obj.result[i].message.from.id);
                        }
                        Params.lastUMid = obj.result[obj.result.Length - 1].message.message_id;
                    }
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    Utilites.cError(e.Message);
                }
            }
        }
    }
}

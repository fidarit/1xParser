using System;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace _1xParser
{
    static class Telegram
    {
        public static Thread msgUpdThread;
        static readonly object paramsUsersLock = new object();

        public static bool EditMessage(string text, int targetID, int msgID)
        {
            string eText = HttpUtility.UrlEncode(text, Encoding.UTF8);

            eText = Utilites.Post("https://api.telegram.org/bot" + Params.TelegToken + "/editMessageText",
                "chat_id=" + targetID + "message_id=" + msgID + "&text=" + eText);

            return eText.Length > 10;
        }
        public static int SendMessage(string text, int targetID)
        {
            string eText = HttpUtility.UrlEncode(text, Encoding.UTF8);

            eText = Utilites.Post("https://api.telegram.org/bot" + Params.TelegToken + "/sendMessage",
                "chat_id=" + targetID + "&text=" + eText);

            if (eText.Length > 10)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                jsonFormats.SendMsgResRoot obj = serializer.Deserialize<jsonFormats.SendMsgResRoot>(eText);

                if (obj.result != null && obj.result.message_id >= 0)
                    return obj.result.message_id;
                else
                    return -1;
            }
            else
                return -1;
        }
        public static bool SendMessageToEveryone(string text)
        {
            bool ret = true;
            lock (paramsUsersLock)
            {
                for (int i = 0; i < Params.Users.Count; i++)
                {
                    ret &= SendMessage(text, Params.Users[i]) != -1;
                }
            }
            return ret;
        }
        public static bool SendMessagesFromAlgs(string text, int algoritm, int gameID)
        {
            bool ret = true;
            lock (paramsUsersLock)
            {
                for (int i = 0; i < Params.Users.Count; i++)
                {
                    int result = SendMessage(text, Params.Users[i]);
                    if (result != -1)
                    {
                        Message message = new Message() { 
                            chatID = Params.Users[i],
                            msgID = result
                        };
                        Program.games[gameID].algoritms[algoritm - 1].messages.Add(message);
                        Program.games[gameID].algoritms[algoritm - 1].messageText = text;
                    }
                    else
                        ret &= false;
                }
            }
            return ret;
        }
        public static void StartMsgUpd()
        {
            if (msgUpdThread == null || !msgUpdThread.IsAlive)
            {
                msgUpdThread = new Thread(MsgUpd);
                msgUpdThread.Name += " Telegram Messages Upd";
                msgUpdThread.Start();
            }
        }
        static void MsgUpd()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            while (TasksMgr.doOtherThreads)
            {
                try
                {
                    string offset = Params.LastUMid == -1 ? "" : "?offset=" + (Params.LastUMid + 1).ToString();
                    string ret = Utilites.GET("https://api.telegram.org/bot" + Params.TelegToken + "/getUpdates" + offset);
                    jsonFormats.GetUpdResRoot obj = serializer.Deserialize<jsonFormats.GetUpdResRoot>(ret);

                    if (obj != null && obj.ok && obj.result.Length > 0)
                    {
                        for (int i = 0; i < obj.result.Length; i++)
                        {
                            jsonFormats.Result result = obj.result[i];
                            int id = result.message.from.id;
                            if (result.message.message_id > Params.LastUMid)
                            {
                                switch (result.message.text)
                                {
                                    case "Старт":
                                    case "cтарт":
                                    case "/start":
                                        if (Params.Users.Contains(id))
                                        {
                                            SendMessage("Для вас уже включена рассылка", id);
                                        }
                                        else
                                        {
                                            Params.Users.Add(id);
                                            SendMessage("Теперь вы будете получать рассылку", id);
                                            Utilites.Log(result.message.from.first_name + " добавлен в список пользователей");
                                        }
                                        break;
                                    case "Стоп":
                                    case "cтоп":
                                    case "/stop":
                                        if (Params.Users.Contains(id))
                                        {
                                            Params.Users.Remove(id);
                                            SendMessage("Теперь вы не будете получать рассылку", id);
                                            Utilites.Log(result.message.from.first_name + " удалён из списка пользователей");
                                        }
                                        else
                                        {
                                            SendMessage("Вы и так не получаете рассылку...", id);
                                        }
                                        break;
                                    default:
                                        SendMessage("Извините, но пока я вас не понимаю...", id);
                                        Utilites.LogWarning(result.message.from.first_name + " пишет: " + result.message.text);
                                        break;
                                }
                            }
                        }
                        Params.LastUMid = obj.result[obj.result.Length - 1].message.message_id;
                    }
                    Thread.Sleep(1500);
                }
                catch (Exception e)
                {
                    Utilites.LogException(e);
                }
            }
        }
    }
}

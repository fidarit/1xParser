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
            string resultText = Utilites.Post("https://api.telegram.org/bot"
                + Settings.TelegToken + "/editMessageText",
                "chat_id=" + targetID + 
                "&message_id=" + msgID + 
                "&text=" + HttpUtility.UrlEncode(text, Encoding.UTF8));

            return resultText.Length > 10;
        }
        public static int SendMessage(string text, int targetID)
        {
            string resultText = Utilites.Post("https://api.telegram.org/bot"
                + Settings.TelegToken + "/sendMessage",
                "chat_id=" + targetID + 
                "&text=" + HttpUtility.UrlEncode(text, Encoding.UTF8));

            if (resultText.Length < 10)
                return -1;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var obj = serializer.Deserialize<jsonFormats.SendMsgResRoot>(resultText);

            return obj?.result.message_id ?? -1;
        }
        public static bool SendMessageToEveryone(string text)
        {
            bool ret = true;
            lock (paramsUsersLock)
            {
                for (int i = 0; i < Settings.GetUsers.Count; i++)
                {
                    ret &= SendMessage(text, Settings.GetUsers[i]) != -1;
                }
            }
            return ret;
        }
        public static bool SendMessagesFromAlgorithms(string text, int algoritm, int gameID)
        {
            bool retResult = false;
            lock (paramsUsersLock)
            {
                if (Settings.GetUsers.Count == 0)
                    return true;

                Program.games[gameID].algoritms[algoritm - 1].messageText = text;

                for (int i = 0; i < Settings.GetUsers.Count; i++)
                {
                    int result = SendMessage(text, Settings.GetUsers[i]);
                    if (result != -1)
                    {
                        Message message = new Message()
                        {
                            chatID = Settings.GetUsers[i],
                            msgID = result
                        };
                        Program.games[gameID].algoritms[algoritm - 1].messages.Add(message);

                        retResult |= true;
                    }
                }
            }
            return retResult;
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

            while (TaskManager.doOtherThreads)
            {
                try
                {
                    string offset = Settings.LastUMid == -1 ? "" : "offset=" + (Settings.LastUMid + 1);
                    string ret = Utilites.Post("https://api.telegram.org/bot"
                        + Settings.TelegToken + "/getUpdates", offset);

                    var obj = serializer.Deserialize<jsonFormats.GetUpdResRoot>(ret);

                    if (obj != null && obj.ok && obj.result.Length > 0)
                    {
                        for (int i = 0; i < obj.result.Length; i++)
                            ProcessMessage(obj.result[i]);

                        Settings.LastUMid = obj.result[obj.result.Length - 1].message.message_id;
                    }
                    Thread.Sleep(1500);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        private static void ProcessMessage(jsonFormats.Result result)
        {
            int id = result.message.from.id;

            if (result.message.message_id > Settings.LastUMid)
            {
                switch (result.message.text)
                {
                    case "Старт":
                    case "cтарт":
                    case "/start":
                        if (Settings.GetUsers.Contains(id))
                        {
                            SendMessage("Для вас уже включена рассылка", id);
                        }
                        else
                        {
                            Settings.GetUsers.Add(id);
                            SendMessage("Теперь вы будете получать рассылку", id);
                            Debug.Log(result.message.from.first_name
                                + " добавлен в список пользователей");
                        }
                        break;
                    case "Стоп":
                    case "cтоп":
                    case "/stop":
                        if (Settings.GetUsers.Contains(id))
                        {
                            Settings.GetUsers.Remove(id);
                            SendMessage("Теперь вы не будете получать рассылку", id);
                            Debug.Log(result.message.from.first_name
                                + " удалён из списка пользователей");
                        }
                        else
                        {
                            SendMessage("Вы и так не получали рассылку...", id);
                        }
                        break;
                    default:
                        SendMessage("Извините, но я вас не понимаю...", id);
                        Debug.LogWarning(result.message.from.first_name
                            + " пишет: " + result.message.text);
                        break;
                }
            }
        }
    }
}

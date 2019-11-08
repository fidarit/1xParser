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

        public static int SendMessage(string text, int targetID)
        {
            string eText = HttpUtility.UrlEncode(text, Encoding.UTF8);
            DateTime time = DateTime.Now;

            eText = Utilites.Post("https://api.telegram.org/bot" + Params.TelegToken + "/sendMessage",
                "chat_id=" + targetID + "&text=" + eText);

            int sleepTime = (int)(time.AddMilliseconds(33) - DateTime.Now).TotalMilliseconds;
            if (sleepTime > 0) Thread.Sleep(sleepTime);

            if (eText.Length > 10)
                return 0;
            else
                return 1;
        }
        public static int SendMessageToAll(string text)
        {
            int ret = Params.Users.Count;
            for (int i = 0; i < Params.Users.Count; i++)
            {
                ret *= SendMessage(text, Params.Users[i]);
            }

            return ret;
        }
        public static void StartMsgUpd()
        {
            msgUpdThread = new Thread(MsgUpd);
            msgUpdThread.Start();
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

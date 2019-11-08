using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace _1xParser
{
    class Program
    {
        public static Dictionary<int, Game> games;
        public static readonly object gamesLocker = new object();

        static bool doItAll = true;

        static int Main()
        {
            handlerRoutine = new HandlerRoutine(ConsoleCtrlCheck);
            SetConsoleCtrlHandler(handlerRoutine, true);

            while (doItAll)
            {
                try
                {
                    Utilites.Log("Запуск программы");
                    games = new Dictionary<int, Game>();
                    
                    if (!Params.LoadParams())
                    {
                        Utilites.LogError("Файл настроек не обнаружен - \"params.xml\", создаю новый...");
                    }
                    if (!Params.LoadUsers())
                    {
                        Utilites.LogError("Файл со списком пользователей не обнаружен  - \"users.xml\", создаю новый...");
                    }
                    if (Params.TelegToken == null || Params.TelegToken.Length < 6)
                    {
                        Utilites.LogError("Токен Telegram API не обнаружен, надо ввести его в файле params.xml");
                        Utilites.LogWarning("Завершение работы...");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
                        return -1;
                    }

                    Utilites.Log("Нажмите Ctrl + C чтобы сохранить список пользователей");

                    Utilites.Log("Запускаю сервисы Telegram");
                    Telegram.StartMsgUpd();

                    TasksMgr.StartLineParsing();
                    TasksMgr.StartUsersSaving();

                    Thread.Sleep(12000000); // ждать 3.333 часа
                    bool sleepAgain = false;
                    while (!sleepAgain)
                    {
                        foreach (Game game in games.Values)
                        {
                            sleepAgain |= game.algActived[0] | game.algActived[1] | game.algActived[2]; //если есть активные алгоритмы
                            int time = game.startTimeUNIX - Utilites.NowUNIX();
                            sleepAgain |= time > 0 && time < 300; //или в течении пяти минут начнется игра
                            if (sleepAgain)
                            {
                                Thread.Sleep(500000); //то подождать ещё 8,333 мин
                                break;
                            }
                        }
                    }

                }
                catch(Exception e)
                {
                    Utilites.LogException(e);                    
                }
                finally
                {
                    Close();
                }
            }
            return 0;
        }
        static void Close()
        {
            Utilites.Log("Остановка фоновых задач");
            TasksMgr.doOtherThreads = false;

            if (TasksMgr.taskThread != null)
                if(TasksMgr.taskThread.ThreadState == ThreadState.WaitSleepJoin)
                    TasksMgr.taskThread.Interrupt();
                else if (TasksMgr.taskThread.IsAlive)
                    TasksMgr.taskThread.Abort();

            if (TasksMgr.parsingThread != null)
                if (TasksMgr.parsingThread.ThreadState == ThreadState.WaitSleepJoin)
                    TasksMgr.parsingThread.Interrupt();
                else if (TasksMgr.parsingThread.IsAlive)
                    TasksMgr.parsingThread.Abort();

            if (Telegram.msgUpdThread != null)
            {
                if (Telegram.msgUpdThread.ThreadState == ThreadState.WaitSleepJoin)
                    Telegram.msgUpdThread.Interrupt();
                else if (Telegram.msgUpdThread.IsAlive && !Telegram.msgUpdThread.Join(2500))
                    Telegram.msgUpdThread.Abort();

                Telegram.msgUpdThread.Join();
            }

            Params.SaveUsers();

            Utilites.Log("Завершение работы");
            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();
        }

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        private static HandlerRoutine handlerRoutine;
        private static GCHandle gcObjKeyboardProcess = GCHandle.Alloc(handlerRoutine);
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        [DllImport("Kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetConsoleCtrlHandler(
            [In]
            [Optional]
            [MarshalAs(UnmanagedType.FunctionPtr)]
            HandlerRoutine Handler,
            [In]
            [MarshalAs(UnmanagedType.Bool)]
            bool Add
          );
        static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_CLOSE_EVENT:
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    doItAll = false;
                    Close();
                    Thread.Sleep(1500);
                    return true;
                case CtrlTypes.CTRL_C_EVENT:
                    Params.SaveUsers();
                    return true;
                default:
                    return false;
            }
        }
    }
}

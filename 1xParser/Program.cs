using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
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
#if DEBUG
#else
            Console.OutputEncoding = Encoding.UTF8;
#endif
            while (doItAll)
            {
                try
                {
                    Debug.Log("Запуск программы");
                    games = new Dictionary<int, Game>();
                    TaskManager.doOtherThreads = true;

                    if (!Params.LoadParams())
                    {
                        Debug.LogError("Файл настроек не обнаружен - \"params.xml\", создаю новый...");
                    }
                    if (!Params.LoadUsers())
                    {
                        Debug.LogError("Файл со списком пользователей не обнаружен  - \"users.xml\", создаю новый...");
                    }
                    if (Params.TelegToken == null || Params.TelegToken.Length < 6)
                    {
                        Debug.LogError("Токен Telegram API не обнаружен, надо ввести его в файле params.xml");
                        Debug.LogWarning("Завершение работы...");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
                        return -1;
                    }
                    
                    Debug.Log("Нажмите Ctrl + C чтобы сохранить список пользователей");

                    Debug.Log("Запускаю сервисы Telegram");
                    Telegram.StartMsgUpd();

                    TaskManager.StartLineParsing();
                    TaskManager.StartUsersSavingThread();

                    Thread.Sleep(6 * 3600 * 1000); //ждать 6 часов
                    bool sleepAgain = true;
                    while (sleepAgain)
                    {
                        sleepAgain = false;
                        lock (gamesLocker)
                        {
                            foreach (Game game in games.Values)
                            {   //если есть активные алгоритмы
                                sleepAgain |= game.algoritms[0].actived;
                                sleepAgain |= game.algoritms[1].actived;
                                sleepAgain |= game.algoritms[2].actived;
                                sleepAgain |= game.deleteFuncIsActivated;
                            }
                        }
                        if (sleepAgain)
                            Thread.Sleep(600000); //то подождать ещё 10 мин
                    }
                }
                catch(Exception e)
                {
                    Debug.LogException(e);                    
                }
                finally
                {
                    TaskManager.PrefClosing();
                }
            }
            return 0;
        }

        //Весь код ниже взаимствован из StackOverflow
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
                    TaskManager.PrefClosing();
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

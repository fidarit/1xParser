using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace _1xParser
{
    class Program
    {
        public static Dictionary<long, Game> games;
        static bool doItAll = true;

        static int Main(string[] args)
        {
            SetConsoleCtrlHandler(ConsoleCtrlCheck, true);

            while (doItAll)
            {
                try
                {
                    Utilites.cMsg("Starting");
                    games = new Dictionary<long, Game>();


                    Utilites.cMsg("Load Settings");
                    if (!Params.LoadParams())
                    {
                        Utilites.cError("There is not settings file  - \"params.xml\"");
                        Utilites.cWarning("Settings file created");
                        Utilites.cWarning("Closing");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
                        return -1;
                    }
                    Utilites.cMsg("Press Ctrl + C to settings saving");

                    Utilites.cMsg("Starting Telegram messages updater");
                    Telegram.StartMsgUpd();

                    tasksMgr.StartLineParsing();

                    Thread.Sleep((int)TimeSpan.FromHours(6).TotalMilliseconds); //Restart program every 6 hours
                }
                catch(Exception e)
                {
                    Utilites.wrException(e);                    
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
            Utilites.cMsg("Stopping background tasks");
            tasksMgr.doOtherThreads = false;

            if (tasksMgr.taskThread != null)
                if(tasksMgr.taskThread.ThreadState == ThreadState.WaitSleepJoin)
                    tasksMgr.taskThread.Interrupt();
                else if (tasksMgr.taskThread.IsAlive)
                    tasksMgr.taskThread.Abort();

            if (tasksMgr.parsingThread != null)
                if (tasksMgr.parsingThread.ThreadState == ThreadState.WaitSleepJoin)
                    tasksMgr.parsingThread.Interrupt();
                else if (tasksMgr.parsingThread.IsAlive)
                    tasksMgr.parsingThread.Abort();

            if (Telegram.msgUpdThread != null)
            {
                if (Telegram.msgUpdThread.ThreadState == ThreadState.WaitSleepJoin)
                    Telegram.msgUpdThread.Interrupt();
                else if (Telegram.msgUpdThread.IsAlive && !Telegram.msgUpdThread.Join(2500))
                    Telegram.msgUpdThread.Abort();

                Telegram.msgUpdThread.Join();
            }

            Params.SaveParams();

            Utilites.cMsg("Closing");
            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();
        }

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
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
                    Params.SaveParams();
                    return true;
                default:
                    return false;
            }
        }
    }
}

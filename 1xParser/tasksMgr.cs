using System;
using System.Collections.Generic;
using System.Threading;

namespace _1xParser
{
    struct Task
    {
        public int TimeUNIX { get; set; }
        public int GameID { get; set; }
        public Action<int> Func { get; set; }
    }
    static class TasksMgr
    {
        static readonly List<Task> tasks = new List<Task>();
        static readonly object tasksLocker = new object();

        public static bool doOtherThreads;
        public static Thread taskThread;
        public static Thread parsingThread;
        public static Thread usersSavingThread;

        public static void StartLineParsing()
        {
            if (parsingThread == null || !parsingThread.IsAlive)
            {
                parsingThread = new Thread(LineParsing);
                parsingThread.Name += " Line Parsing Thread";
                parsingThread.Start();
            }
        }
        public static void AddTask(Task task)
        {
            lock (tasksLocker)
            {
                if(tasks.Count > 0 && task.TimeUNIX < tasks[0].TimeUNIX && taskThread != null
                    && taskThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    taskThread.Abort();
                    taskThread = null;
                }
                if (taskThread == null || !taskThread.IsAlive)
                {
                    taskThread = new Thread(DoIt);
                    taskThread.Name += " Task Thread";
                    taskThread.Start();
                }

                tasks.Add(task);
                tasks.Sort((a, b) => a.TimeUNIX.CompareTo(b.TimeUNIX));
            }
        }
        public static void StartUsersSaving()
        {
            if (usersSavingThread == null || !usersSavingThread.IsAlive)
            {
                usersSavingThread = new Thread(UsersSaving);
                usersSavingThread.Name += " Users Saving Thread";
                usersSavingThread.Start();

            }
        }
        static void LineParsing()
        {
            try
            {
                while (doOtherThreads)
                {
                    Parser.ParseLine();

                    Thread.Sleep(200000); //3 min 20 sec
                }
            }
            catch (Exception e)
            {
                Utilites.LogException(e);
            }
        }
        static void UsersSaving()
        {
            try
            {
                Thread.Sleep(30000); //30 sec
                while (doOtherThreads)
                {
                    Params.SaveUsers();
                    Thread.Sleep(600000); //10 min
                }
            }
            catch (Exception e)
            {
                Utilites.LogException(e);
            }
        }
        public static void PrefClosing()
        {
            Utilites.Log("Остановка фоновых задач");
            doOtherThreads = false;
            Program.games.Clear();
            tasks.Clear();

            if (taskThread != null)
                if (taskThread.ThreadState == ThreadState.WaitSleepJoin)
                    taskThread.Interrupt();
                else if (taskThread.IsAlive)
                    taskThread.Abort();

            if (parsingThread != null)
                if (parsingThread.ThreadState == ThreadState.WaitSleepJoin)
                    parsingThread.Interrupt();
                else if (parsingThread.IsAlive)
                    parsingThread.Abort();

            if (Telegram.msgUpdThread != null)
            {
                if (Telegram.msgUpdThread.ThreadState == ThreadState.WaitSleepJoin)
                    Telegram.msgUpdThread.Interrupt();
                else if (Telegram.msgUpdThread.IsAlive && !Telegram.msgUpdThread.Join(2500))
                    Telegram.msgUpdThread.Abort();

                if (Telegram.msgUpdThread.IsAlive)
                {
                    Telegram.msgUpdThread.Join();
                }
            }
            if (usersSavingThread != null)
            {
                if (usersSavingThread.ThreadState == ThreadState.WaitSleepJoin)
                    usersSavingThread.Interrupt();
                else if (usersSavingThread.IsAlive && !usersSavingThread.Join(2500))
                    usersSavingThread.Abort();

                if (usersSavingThread.IsAlive)
                {
                    usersSavingThread.Join();
                }
            }

            Params.SaveUsers();

            Utilites.Log("Завершение работы");
        }
        static void DoIt()
        {
            try
            {
                while (doOtherThreads)
                {
                    Task task;
                    lock (tasksLocker)
                    {
                        task = tasks[0];
                    }
                    int sleepTime = task.TimeUNIX - Utilites.NowUNIX();

                    if (sleepTime > 0) Thread.Sleep(sleepTime * 1000);

                    try
                    {
                        task.Func(task.GameID);
                    }
                    catch (Exception e)
                    {
                        Utilites.LogException(e);
                    }

                    lock (tasksLocker)
                    {
                        tasks.Remove(task);
                        if (tasks.Count == 0) return;
                    }
                }
            }
            catch (Exception e)
            {
                Utilites.LogException(e);
            }
        }
    }
}

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

        static bool parsingStarted = false;
        static readonly EventWaitHandle parsingThrEv = new EventWaitHandle(false, EventResetMode.ManualReset);
        public static bool doOtherThreads = true;
        public static Thread taskThread;
        public static Thread parsingThread;
        public static Thread usersSavingThread;

        public static void StartLineParsing()
        {
            if (!parsingStarted)
            {
                if (parsingThread == null)
                {
                    parsingThread = new Thread(LineParsing);
                    parsingThread.Name += " Line Parsing Thread";
                }
                else parsingThrEv.Set();
                if (!parsingThread.IsAlive) parsingThread.Start();
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
            usersSavingThread = new Thread(UsersSaving);
            usersSavingThread.Name += " Users Saving Thread";
            usersSavingThread.Start();
        }
        static void LineParsing()
        {
            try
            {
                while (doOtherThreads)
                {
                    parsingStarted = true;

                    Parser.ParseLine();

                    parsingStarted = false;
                    parsingThrEv.WaitOne(200000); //3 min 20 sec
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
                        if (task.Func != null)
                            task.Func(task.GameID);
                        else
                            parsingThrEv.Set();

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

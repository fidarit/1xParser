using System;
using System.Collections.Generic;
using System.Threading;

namespace _1xParser
{
    struct Task
    {
        public int timeUTC { get; set; }
        public long gameID { get; set; }
        public Action<long> func { get; set; }
    }
    static class tasksMgr
    {
        static List<Task> tasks = new List<Task>();
        static bool taskStarted = false;
        static bool parsingStarted = false;
        static EventWaitHandle parsingThrEv = new EventWaitHandle(false, EventResetMode.ManualReset);
        public static bool doOtherThreads = true;
        public static Thread taskThread;
        public static Thread parsingThread;
        public static Thread paramsSavingThread;

        public static void StartLineParsing()
        {
            if (!parsingStarted)
            {
                if (parsingThread == null)
                {
                    parsingThread = new Thread(lineParsing);
                    parsingThread.Name += " Line Parsing Thread";
                }
                else parsingThrEv.Set();
                if (!parsingThread.IsAlive) parsingThread.Start();
            }
        }
        public static void AddTask(Task task)
        {
            tasks.Add(task);
            tasks.Sort((a, b) => a.timeUTC.CompareTo(b.timeUTC));
            if (!taskStarted)
            {
                if (taskThread == null)
                {
                    taskThread = new Thread(doIt);
                    taskThread.Name += " Task Thread";
                }
                if (!taskThread.IsAlive) taskThread.Start();
            }
        }
        public static void StartParamsSaving()
        {
            paramsSavingThread = new Thread(paramsSaving);
            paramsSavingThread.Name += " Settings Saving Thread";
            paramsSavingThread.Start();
        }
        static void lineParsing()
        {
            try
            {
                while (doOtherThreads)
                {
                    parsingStarted = true;

                    Parser.ParseLine();

                    parsingStarted = false;
                    parsingThrEv.WaitOne(300000);
                }
            }
            catch (Exception e)
            {
                Utilites.wrException(e);
            }
        }
        static void paramsSaving()
        {
            try
            {
                while (doOtherThreads)
                {
                    Params.SaveParams();

                    Thread.Sleep(6000000); //10 min
                }
            }
            catch (Exception e)
            {
                Utilites.wrException(e);
            }
        }
        static void doIt()
        {
            try
            {
                while (doOtherThreads)
                {
                    taskStarted = true;

                    int sleepTime = (int)(tasks[0].timeUTC - DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds);
                    if (sleepTime > 1000) Thread.Sleep(sleepTime);

                    if (tasks[0].func != null)
                        tasks[0].func(tasks[0].gameID);
                    else
                        parsingThrEv.Set();

                    tasks.RemoveAt(0);
                    taskStarted = false;
                    if (tasks.Count == 0) return;
                }
            }
            catch (Exception e)
            {
                Utilites.wrException(e);
            }
        }
    }
}

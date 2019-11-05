using System;
using System.Collections.Generic;
using System.Threading;

namespace _1xParser
{
    struct Task
    {
        public int TimeUNIX { get; set; }
        public long GameID { get; set; }
        public Action<long> Func { get; set; }
    }
    static class TasksMgr
    {
        static readonly List<Task> tasks = new List<Task>();
        static bool parsingStarted = false;
        static readonly EventWaitHandle parsingThrEv = new EventWaitHandle(false, EventResetMode.ManualReset);
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
                    parsingThread = new Thread(LineParsing);
                    parsingThread.Name += " Line Parsing Thread";
                }
                else parsingThrEv.Set();
                if (!parsingThread.IsAlive) parsingThread.Start();
            }
        }
        public static void AddTask(Task task)
        {
            tasks.Add(task);
            tasks.Sort((a, b) => a.TimeUNIX.CompareTo(b.TimeUNIX));
            if (taskThread == null || !taskThread.IsAlive)
            {
                taskThread = new Thread(DoIt);
                taskThread.Name += " Task Thread";
                taskThread.Start();
            }
        }
        public static void StartParamsSaving()
        {
            paramsSavingThread = new Thread(ParamsSaving);
            paramsSavingThread.Name += " Settings Saving Thread";
            paramsSavingThread.Start();
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
                    parsingThrEv.WaitOne(300000);
                }
            }
            catch (Exception e)
            {
                Utilites.LogException(e);
            }
        }
        static void ParamsSaving()
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
                Utilites.LogException(e);
            }
        }
        static void DoIt()
        {
            try
            {
                while (doOtherThreads)
                {
                    int sleepTime = tasks[0].TimeUNIX - Utilites.NowUNIX();
                    if (sleepTime > 0) Thread.Sleep(sleepTime * 1000);

                    try
                    {
                        if (tasks[0].Func != null)
                            tasks[0].Func(tasks[0].GameID);
                        else
                            parsingThrEv.Set();

                    }
                    catch (Exception e)
                    {
                        Utilites.LogException(e);
                    }

                    tasks.RemoveAt(0);
                    if (tasks.Count == 0) return;
                }
            }
            catch (Exception e)
            {
                Utilites.LogException(e);
            }
        }
    }
}

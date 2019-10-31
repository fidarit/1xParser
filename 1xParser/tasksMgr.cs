using System;
using System.Collections.Generic;
using System.Threading;

namespace _1xParser
{
    delegate void taskFunc(ulong id);
    struct Task
    {
        public DateTime dt { get; set; }
        public ulong gameID { get; set; }
        public taskFunc func { get; set; }
    }
    static class tasksMgr
    {
        static List<Task> tasks = new List<Task>();
        static bool taskStarted = false;
        public static bool doOtherThreads = true;
        public static Thread taskThread;
        public static void AddTask(Task task)
        {
            tasks.Add(task);
            if (!taskStarted)
            {
                if(taskThread == null) taskThread = new Thread(doIt);
                taskThread.Start();
            }
        }
        static void doIt()
        {
            try
            {
                while (doOtherThreads)
                {
                    taskStarted = true;

                    int sleepTime = tasks[0].dt.Millisecond - DateTime.Now.Millisecond;
                    if (sleepTime > 10000) Thread.Sleep(sleepTime);

                    tasks[0].func(tasks[0].gameID);

                    tasks.RemoveAt(0);
                    taskStarted = false;
                    if (tasks.Count == 0) return;
                }
            }
            catch (Exception e)
            {
                Utilites.cError(e.Message);
            }
        }
    }
}

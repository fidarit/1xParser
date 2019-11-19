using System;
using System.Collections.Generic;

namespace _1xParser
{
    class Game
    {
        public int startTimeUNIX;   //Когда игра начинается
        public int updTimeUNIX;     //Когда последний раз обновляли инфу
        public int gameTime;        //Игровое время
        public bool isFinished;
        public bool deleteFuncIsActivated;
        public string league;

        public double totalF;
        public double totalL;
        public double TkfMore;
        public double TkfLess;

        public double iTotalF;
        public double iTotalL;
        public double iTkfMore;
        public double iTkfLess;
        
        public Algoritm[] algoritms;
        public Team[] teams;
        public short favTeam;       //Команда фаворит для 3-го алгоритма
        public Game()
        {
            teams = new Team[2];

            deleteFuncIsActivated = false;
            algoritms = new Algoritm[3];
            for (int i = 0; i< algoritms.Length; i++)
            {
                algoritms[i].messages = new List<Message>();
                algoritms[i].actived = false;
            }

            favTeam = -1;
            isFinished = false;
        }
        public void AddMsgIDs(Message[] messages, int algoritm)
        {
            if (messages == null)
                return;

            try
            {
                algoritms[algoritm - 1].messages.AddRange(messages);
            }
            catch(Exception e)
            {
                Utilites.LogException(e);
            }
        }
    }
    struct Algoritm
    {
        public List<Message> messages; //Сообщения, которые отправлены алгоритмом
        public string messageText;
        public bool actived;   //Алгоритм работает?
        public bool tMore;
        public double sendedTotal;
    }
    struct Message
    {
        public int msgID;
        public int chatID;
    }
    struct Team
    {
        public string name;
        public double kf;       
        public int goals1T;     //Голы за 1-й тайм
        public int allGoals;    //Все голы
    }
}

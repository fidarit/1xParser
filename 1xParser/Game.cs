using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1xParser
{
    [Serializable]
    class Game
    {
        public int startTimeUNIX;
        public int updTimeUNIX;
        public int gameTime;
        public string league;

        public double totalF;
        public double totalL;
        public double TkfMore;
        public double TkfLess;

        public double iTotalF;
        public double iTotalL;
        public double iTkfMore;
        public double iTkfLess;

        public bool[] algActived;

        public Team[] teams;
        public short favTeam;
        public Game()
        {
            teams = new Team[2];
            algActived = new bool[3] { false, false, false };
            favTeam = -1;
        }
    }
    [Serializable]
    struct Team
    {
        public string name;
        public double kf;
        public int goals1T;
    }
}

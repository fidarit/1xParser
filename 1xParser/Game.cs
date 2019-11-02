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
        public int startTimeUTC;
        public int updTimeUTC;
        public string league;
        public double totalF;
        public double totalL;
        public team[] teams; 
        public Game()
        {
            teams = new team[2];
        }
    }
    [Serializable]
    struct team
    {
        public string name;
    }
}

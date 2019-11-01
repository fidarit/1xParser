using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1xParser
{
    class Game
    {
        public int startTimeUTC;
        public int updTImeUTC;
        public string league;
        public team[] teams; 
        public Game()
        {
            teams = new team[2];
        }
    }
    struct team
    {
        public string name;
    }
}

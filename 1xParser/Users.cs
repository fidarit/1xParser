using System;
using System.Collections.Generic;

namespace _1xParser
{
    [Serializable]
    public class Users
    {
        public int lastUMid = -1;       //Last upd message id
        public uint lastSignalNumer = 0;
        public List<int> users = new List<int>();
        public bool Equals(Users obj)
        {
            bool equals = users.Count == obj.users.Count;
            equals &= lastUMid == obj.lastUMid;
            equals &= lastSignalNumer == obj.lastSignalNumer;
            if (equals)
            {
                foreach (int user in obj.users)
                {
                    equals &= users.Contains(user);
                    if (!equals)
                        break;
                }
            }
            return equals;
        }
    }
}

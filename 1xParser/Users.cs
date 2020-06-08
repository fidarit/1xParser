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
            bool ret = users.Count == obj.users.Count;
            ret &= lastUMid == obj.lastUMid;
            ret &= lastSignalNumer == obj.lastSignalNumer;
            if (ret)
            {
                foreach (int user in obj.users)
                {
                    ret &= users.Contains(user);
                    if (!ret)
                        break;
                }
            }
            return ret;
        }
    }
}

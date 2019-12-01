using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace _1xParser
{
    static class Params
    {
        static ParamsObj m_params;

        static UsersObj m_users;
        static UsersObj lastSavedUsers;

        const string paramsFile = "params.xml";
        const string usersFile = "users.xml";
        const string backupDir = "Backups";

        public static string TelegToken
        {
            get { return m_params.telegToken; }
            set { m_params.telegToken = value; }
        }
        public static List<int> Users
        {
            get { return m_users.users; }
            set { m_users.users = value; }
        }
        public static int LastUMid
        {
            get { return m_users.lastUMid; }
            set { m_users.lastUMid = value; }
        }
        public static uint LastSignalNumer
        {
            get { return m_users.lastSignalNumer; }
            set { m_users.lastSignalNumer = value; }
        }
        public static bool UseProxy
        {
            get { return m_params.useProxy; }
            set { m_params.useProxy = value; }
        }
        public static string ProxyIP
        {
            get { return m_params.proxyIP; }
            set { m_params.proxyIP = value; }
        }
        public static int ProxyPort
        {
            get { return m_params.proxyPort; }
            set { m_params.proxyPort = value; }
        }

        public static bool LoadParams()
        {
            Utilites.Log("Загрузка настроек");
            if (File.Exists(paramsFile))
            {
                try
                {
                    using (FileStream fs = new FileStream(paramsFile, FileMode.Open))
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(ParamsObj));
                        m_params = (ParamsObj)formatter.Deserialize(fs);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Utilites.LogException(e);
                    return false;
                }
            }
            else
            {
                m_params = new ParamsObj();

                XmlSerializer formatter = new XmlSerializer(typeof(ParamsObj));
                using (FileStream fs = new FileStream(paramsFile, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, m_params);
                }

                return false;
            }
        }
        public static bool LoadUsers(byte id = 0)
        {
            if (File.Exists(usersFile))
            {
                try
                {
                    using (FileStream fs = new FileStream(usersFile, FileMode.Open))
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(UsersObj));
                        m_users = (UsersObj)formatter.Deserialize(fs);
                    }
                    using (FileStream fs = new FileStream(usersFile, FileMode.Open))
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(UsersObj));
                        lastSavedUsers = (UsersObj)formatter.Deserialize(fs);
                    }
                    return true;
                }
                catch(Exception e)
                {
                    int i = 0;
                    string errFile = usersFile + ".err";
                    while (File.Exists(errFile + i))
                    {
                        if (++i > 9)
                            break;
                    }
                    File.Move(usersFile, errFile + i);
                    Utilites.LogException(e);
                    return LoadBackup(id);
                }
            }
            else
            {
                if (LoadBackup(id))
                    return true;
                else
                {
                    m_users = new UsersObj();
                    SaveUsers();
                }

                return false;
            }
        }
        public static void SaveUsers()
        {
            if (File.Exists(usersFile))
            {
                if (m_users == null || (lastSavedUsers != null && lastSavedUsers.Equals(m_users)))
                    return;

                Utilites.Log("Сохраняю список пользователей...");
                string backupFile = backupDir + "/" + usersFile + ".";
                Directory.CreateDirectory(backupDir);

                List<string> files = new List<string>(Directory.GetFiles(backupDir, "*.xml.*"));

                if (files.Count > 9)
                {
                    backupFile += "9";
                    if (File.Exists(backupFile))
                        File.Delete(backupFile);
                }
                else
                {
                    string last = "";

                    if (files.Count > 0)
                    {
                        files.Sort((a, b) => a.CompareTo(b));
                        files.Reverse();
                        last = files[0].Remove(0, files[0].LastIndexOf(".") + 1);
                    }

                    if (!int.TryParse(last, out int lastInt))
                    {
                        backupFile += "0";
                    }
                    else
                    {
                        backupFile += lastInt + 1;
                    }
                }

                File.Move(usersFile, backupFile);
            }

            XmlSerializer formatter = new XmlSerializer(typeof(UsersObj));
            using (FileStream fs = new FileStream(usersFile, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, m_users);
            }
            using (FileStream fs = new FileStream(usersFile, FileMode.OpenOrCreate))
            {
                lastSavedUsers = (UsersObj)formatter.Deserialize(fs);
            }
        }

        static bool LoadBackup(byte id = 0)
        {
            if (Directory.Exists(backupDir) && id < 10)
            {
                List<string> files = new List<string>(Directory.GetFiles(backupDir, usersFile + ".*"));

                if (files.Count > 0)
                {
                    Utilites.LogWarning("Загружаю бекап файла списка пользователей");
                    files.Sort((a, b) => a.CompareTo(b));
                    files.Reverse();

                    File.Copy(files[id], usersFile);
                    id++;
                    return LoadUsers(id);
                }
            }
            return false;
        }
    }
    [Serializable]
    public class ParamsObj
    {
        public string telegToken = "Сюда";

        public bool useProxy = false;
        public string proxyIP = "0.0.0.0";
        public int proxyPort = 1234;

        public bool Equals(ParamsObj obj)
        {
            return telegToken == obj.telegToken
                && useProxy == obj.useProxy
                && proxyIP == obj.proxyIP
                && proxyPort == obj.proxyPort;
        }
    }
    [Serializable]
    public class UsersObj
    {
        public int lastUMid = -1;       //Last upd message id
        public uint lastSignalNumer = 0;
        public List<int> users = new List<int>();
        public bool Equals(UsersObj obj)
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

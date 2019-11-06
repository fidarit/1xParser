using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace _1xParser
{
    static class Params
    {
        static ParamsObj m_params;
        static ParamsObj lastSavedParams;
        const string paramsFile = "params.xml";
        const string backupDir = "Backups";

        public static string TelegToken
        {
            get { return m_params.telegToken; }
            set { m_params.telegToken = value; }
        }
        public static List<int> Users
        {
            get { return m_params.users; }
            set { m_params.users = value; }
        }
        public static int LastUMid
        {
            get { return m_params.lastUMid; }
            set { m_params.lastUMid = value; }
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
        public static bool LoadParams(byte id = 0)
        {
            if (File.Exists(paramsFile))
            {
                try
                {
                    using (FileStream fs = new FileStream(paramsFile, FileMode.Open))
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(ParamsObj));
                        m_params = (ParamsObj)formatter.Deserialize(fs);
                    }
                    using (FileStream fs = new FileStream(paramsFile, FileMode.Open))
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(ParamsObj));
                        lastSavedParams = (ParamsObj)formatter.Deserialize(fs);
                    }
                    return true;
                }
                catch(Exception e)
                {
                    int i = 0;
                    string errFile = paramsFile + ".err";
                    while (File.Exists(errFile + i))
                    {
                        i++;
                        if (i > 9)
                            break;
                    }
                    File.Move(paramsFile, errFile + i);
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
                    m_params = new ParamsObj();
                    SaveParams();
                }

                lastSavedParams = m_params;
                return false;
            }
        }
        static bool LoadBackup(byte id = 0)
        {
            if (Directory.Exists(backupDir) && id < 10)
            {
                List<string> files = new List<string>(Directory.GetFiles(backupDir, "*.xml.*"));

                if (files.Count > 0)
                {
                    Utilites.LogWarning("Loading Backup");
                    files.Sort((a, b) => a.CompareTo(b));
                    files.Reverse();

                    File.Copy(files[id], paramsFile);
                    id++;
                    return LoadParams(id);
                }
            }
            return false;
        }
        public static void SaveParams()
        {
            Utilites.Log("Saving Settings");
            if (File.Exists(paramsFile))
            {
                if (lastSavedParams.Equals(m_params))
                    return;

                string backupFile = backupDir + "/" + paramsFile + ".";
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

                File.Move(paramsFile, backupFile);
            }

            XmlSerializer formatter = new XmlSerializer(typeof(ParamsObj));
            using (FileStream fs = new FileStream(paramsFile, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, m_params);
            }
            using (FileStream fs = new FileStream(paramsFile, FileMode.OpenOrCreate))
            {
                lastSavedParams = (ParamsObj)formatter.Deserialize(fs);
            }
        }
    }
    [Serializable]
    public class ParamsObj
    {
        public string telegToken = "602929280:AAGde65bQYkgiqEZSD5eoJn2SSIvIOitg90";
        public List<int> users = new List<int>();
        public int lastUMid = -1; //Last upd message id
        public string proxyIP = "";
        public int proxyPort = 0;

        public bool Equals(ParamsObj obj)
        {
            if (telegToken == obj.telegToken
                && users.ToString() == obj.users.ToString()
                && lastUMid == obj.lastUMid
                && proxyIP == obj.proxyIP
                && proxyPort == obj.proxyPort)
            {
                return true;
            }
            else
                return false;
        }
    }
}

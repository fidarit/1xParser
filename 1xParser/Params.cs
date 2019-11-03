using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace _1xParser
{
    static class Params
    {
        static m_Params m_params;
        static m_Params lastSavedParams;
        const string paramsFile = "params.xml";
        const string backupDir = "Backups";

        public static string telegToken
        {
            get { return m_params.telegToken; }
            set { m_params.telegToken = value; }
        }
        public static List<int> users
        {
            get { return m_params.users; }
            set { m_params.users = value; }
        }
        public static int lastUMid
        {
            get { return m_params.lastUMid; }
            set { m_params.lastUMid = value; }
        }
        public static string proxyIP
        {
            get { return m_params.proxyIP; }
            set { m_params.proxyIP = value; }
        }
        public static int proxyPort
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
                        XmlSerializer formatter = new XmlSerializer(typeof(m_Params));
                        m_params = (m_Params)formatter.Deserialize(fs);
                    }
                    using (FileStream fs = new FileStream(paramsFile, FileMode.Open))
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(m_Params));
                        lastSavedParams = (m_Params)formatter.Deserialize(fs);
                    }
                    return true;
                }
                catch(Exception e)
                {
                    Utilites.wrException(e);
                    return loadBackup(id);
                }
            }
            else
            {
                if (loadBackup(id))
                    return true;
                else
                {
                    m_params = new m_Params();
                    SaveParams();
                }

                lastSavedParams = m_params;
                return false;
            }
        }
        static bool loadBackup(byte id = 0)
        {
            if (Directory.Exists(backupDir) && id < 10)
            {
                List<string> files = new List<string>(Directory.GetFiles(backupDir, "*.xml.*"));

                if (files.Count > 0)
                {
                    Utilites.cWarning("Loading Backup");
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
            if (lastSavedParams.Equals(m_params))
                return;
            
            Utilites.cMsg("Saving Settings");
            if (File.Exists(paramsFile))
            {
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

            XmlSerializer formatter = new XmlSerializer(typeof(m_Params));
            using (FileStream fs = new FileStream(paramsFile, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, m_params);
            }
            using (FileStream fs = new FileStream(paramsFile, FileMode.OpenOrCreate))
            {
                lastSavedParams = (m_Params)formatter.Deserialize(fs);
            }
        }
    }
    [Serializable]
    public class m_Params
    {
        public string telegToken = "602929280:AAGde65bQYkgiqEZSD5eoJn2SSIvIOitg90";
        public List<int> users = new List<int>();
        public int lastUMid = 0; //Last upd message id
        public string proxyIP = "217.182.51.227";
        public int proxyPort = 8080;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace _1xParser
{
    static class Params
    {
        static m_Params m_params;
        const string paramsFile = "params.xml";

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
        public static bool LoadParams()
        {
            if (File.Exists(paramsFile))
            {
                try
                {
                    using (FileStream fs = new FileStream(paramsFile, FileMode.OpenOrCreate))
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(m_Params));
                        m_params = (m_Params)formatter.Deserialize(fs);
                    }
                    return true;
                }
                catch(Exception e)
                {
                    Utilites.cError(e.Message);
                    return false;
                }
            }
            else
            {
                m_params = new m_Params();
                return false;
            }
        }
        public static void SaveParams()
        {
            using (FileStream fs = new FileStream("params.xml", FileMode.OpenOrCreate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(m_Params));
                formatter.Serialize(fs, m_params);
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

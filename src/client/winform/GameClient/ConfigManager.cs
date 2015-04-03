using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GameClient
{
    public static class ConfigManager
    {
        private static Dictionary<string, string> m_configs;
        static ConfigManager()
        {
            m_configs = new Dictionary<string, string>();
        }
        public static void LoadConfigs()
        {
            Assembly entry = Assembly.GetEntryAssembly();
            string pathOfEntry = Path.GetDirectoryName(entry.Location);
            string[] files = Directory.GetFiles(pathOfEntry, "*.config", SearchOption.TopDirectoryOnly);
            foreach(string f in files)
            {
                try
                {
                    string fileName = Path.GetFileName(f);
                    string prefix = fileName.Substring(0, fileName.LastIndexOf(".config"));
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    xmlDoc.Load(f);
                    XmlElement root = xmlDoc.DocumentElement;
                    var configs = root.SelectNodes("Config");
                    foreach (XmlNode n in configs)
                    {
                        string key = n.Attributes["key"].Value;
                        string value = n.Attributes["value"].Value;
                        m_configs.Add(prefix + "." + key, value);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
        public static string GetConfig(string key)
        {
            string result;
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            m_configs.TryGetValue(key, out result);
            return result;
        }
        public static void ReloadConfig()
        {
            m_configs.Clear();
            LoadConfigs();
        }
    }
}

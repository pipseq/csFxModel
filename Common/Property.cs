using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Common
{
    public class Property
    {
        private static string StartupDir = Environment.CurrentDirectory;
        private Dictionary<string, string> properties = new Dictionary<string, string>();
        private static string propertyFilename = StartupDir + "\\" + "properties.xml";
        private static readonly char[] ListDelimiters = new char[] { ',', '\t', ';' };
        private static readonly char[] ListMapDelimiters = new char[] { '=', ':' };

        private static Property instance;
        private Property()
        {
        }
        public static Property getInstance()
        {
            if (instance == null) instance = new Property();
            return instance;
        }
        #region properties
        public void setProperty(string key, string value)
        {
            if (!properties.ContainsKey(key)) properties.Add(key, value);
            else properties[key] = value;
        }
        public Dictionary<string, double> getMapStringDoubleProperty(string key)
        {
            Dictionary<string, double> map = new Dictionary<string, double>();
            string[] list = getDelimitedListProperty(key);
            foreach (string s in list)
            {
                string[] kv = s.Split(ListMapDelimiters);
                string k = kv[0];
                string v = kv[1];
                double dv = double.Parse(v);
                map.Add(k, dv);
            }
            return map;
        }
        public string[] getDelimitedListProperty(string key)
        {
            string s = getProperty(key);
            return s.Split(ListDelimiters);
        }
        public string getProperty(string key)
        {
            if (properties.ContainsKey(key))
                return properties[key];
            return "";
        }

        public string getProperty(string key, string dflt)
        {
            if (properties.ContainsKey(key))
                return properties[key];
            return dflt;
        }

        public int getIntProperty(string key)
        {
            string s = properties[key];
            return int.Parse(s);
        }

        public int getIntProperty(string key, int dflt)
        {
            if (properties.ContainsKey(key))
            {
                return getIntProperty(key);
            }
            return dflt;
        }

        public double getDoubleProperty(string key, double dflt)
        {
            if (properties.ContainsKey(key))
            {
                return getDoubleProperty(key);
            }
            return dflt;
        }

        public decimal getDecimalProperty(string key, decimal dflt)
        {
            if (properties.ContainsKey(key))
            {
                return getDecimalProperty(key);
            }
            return dflt;
        }

        public DateTime getDateTimeProperty(string key, DateTime dflt)
        {
            if (properties.ContainsKey(key))
            {
                return getDateTimeProperty(key);
            }
            return dflt;
        }

        public bool getBooleanProperty(string key, bool dflt)
        {
            if (properties.ContainsKey(key))
            {
                return getBooleanProperty(key);
            }
            return dflt;
        }

        public double getDoubleProperty(string key)
        {
            string s = properties[key];
            return double.Parse(s);
        }

        public decimal getDecimalProperty(string key)
        {
            string s = properties[key];
            return decimal.Parse(s);
        }

        public bool getBooleanProperty(string key)
        {
            string s = properties[key];
            return bool.Parse(s);
        }

        public DateTime getDateTimeProperty(string key)
        {
            string s = properties[key];
            return DateTime.Parse(s);
        }

        public void saveProperties()
        {
            saveProperties(propertyFilename);
        }

        public void saveProperties(string pfilename)
        {
            StreamWriter sw = new StreamWriter(pfilename);
            sw.WriteLine("<properties>");
            foreach (string key in properties.Keys)
            {
                sw.WriteLine("\t<property key=\"{0}\" value=\"{1}\"/>",
                    key, properties[key]);
            }
            sw.WriteLine("</properties>");
            sw.Close();
        }

        public string[] loadProperties()
        {
            return loadProperties(propertyFilename);
        }
        public string[] loadProperties(string pfilename)
        {
            properties = new Dictionary<string, string>();
            XmlDocument pdoc = new XmlDocument();
            try
            {
                pdoc.Load(pfilename);

            }
            catch (FileNotFoundException e)
            {
                // first time
            }
            catch (Exception e)
            {
                // handleException(e);
                throw e;
            }
            XmlNodeList xnl = pdoc.SelectNodes("/properties/property");
            foreach (XmlNode xn in xnl)
            {
                string key = xn.Attributes["key"].InnerText;
                string value = xn.Attributes["value"].InnerText;
                properties.Add(key, value);
            }
            return new List<string>(properties.Keys).ToArray();
        }
        #endregion

    }
}

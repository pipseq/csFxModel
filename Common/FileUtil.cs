using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Common
{
    public class FileUtil
    {
        public Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>> load(string filename)
        {
            Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>> archive
            = new Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>>();
            load(archive, filename);
            return archive;
        }

        public void load(
            Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>> archive, 
            string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNodeList list = doc.SelectNodes("/data/capture");
            foreach (XmlNode node in list)
            {
                Dictionary<string, Dictionary<string, object>> maps = new Dictionary<string, Dictionary<string, object>>();
                DateTime dt = DateTime.Parse(node.Attributes["timestamp"].Value);
                archive.Add(dt, maps);
                XmlNodeList l2 = node.SelectNodes("*");
                foreach (XmlNode n in l2)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    string symbol = n.Name;
                    maps.Add(symbol, map);
                    XmlNodeList l3 = n.SelectNodes("*");
                    foreach (XmlNode f in l3)
                    {
                        string name = f.Name;
                        string value = f.InnerText;
                        try
                        {
                            map.Add(name, double.Parse(value));
                        }
                        catch (Exception)
                        {
                            try
                            {
                                map.Add(name, DateTime.Parse(value));
                            }
                            catch (Exception)
                            {
                                map.Add(name, value);
                            }

                        }
                    }
                }
            }
        }

        public string reviseFile(string file, string prefix, string revision)
        {
            string path = file.Substring(0, file.LastIndexOf('\\'));
            string root = file.Substring(file.LastIndexOf('\\') + 1, file.LastIndexOf('.') - (file.LastIndexOf('\\') + 1));
            string ext = file.Substring(file.LastIndexOf('.') + 1);
            return string.Format(@"{0}\{1}{2}{3}.{4}", path, prefix, root, revision, ext);
        }

        public void renameFile(string file, string file2)
        {
            try
            {
                File.Move(file, file2);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    string.Format("Cannot rename {0} to {1}", file, file2), ex);
            }
        }


        public void save(
            Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>> archive,
            string file, bool standard, List<string> coreData)
        {
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine("<data>");
                foreach (DateTime dt in archive.Keys)
                {
                    sw.WriteLine("<capture timestamp=\"{0}\">", dt.ToString());
                    save(sw, archive[dt], standard, coreData);
                    sw.WriteLine("</capture>", dt.ToString());
                }
                sw.WriteLine("</data>");
                sw.Close();
            }
        }

        public void save(StreamWriter sw, 
            Dictionary<string, Dictionary<string, object>> maps,
            bool std, List<string> coreData)
        {
            foreach (string k in maps.Keys)
            {
                sw.WriteLine("\t<{0}>", k);
                Dictionary<string, object> map = maps[k];
                foreach (string f in map.Keys)
                {
                    if (!coreData.Contains(f) && std) 
                        continue;
                    sw.WriteLine("\t\t<{0}>{1}</{2}>", f, map[f], f);
                }
                sw.WriteLine("\t</{0}>", k);
            }
        }
    }
}

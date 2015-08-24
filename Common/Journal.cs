using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace Common
{
    public class Journal
    {
        private string file;
        private int attempts = 10;
        private int timeout = 100;
        private char GraphDelimiter = '|';

        public Journal(string file)
        {
            this.file = file;
        }

        public void close()
        {
            // nothing to do yet
        }

        private void writeGraph(FileStream fStream, object graph)
        {
            string json = JsonConvert.SerializeObject(graph, Formatting.Indented);
            StreamWriter sw = new StreamWriter(fStream);
            sw.Write(json);
            sw.Write(GraphDelimiter);
            sw.Flush();
            sw.Close();
        }

        private void writer(object graph, FileMode fileMode)
        {
            if (file == null)
                return;
            for (int i = 0; i < attempts; i++) try
                {
                    using (FileStream fStream
                        = File.Open(file,
                        fileMode,
                        FileAccess.Write,
                        FileShare.None))
                    {
                        writeGraph(fStream, graph);
                    }
                    break;
                }
                catch (IOException ioe)
                {
                    Thread.Sleep(timeout);
                }
        }
        public void write(object graph)
        {
            writer(graph, FileMode.Create);
        }

        public void append(object graph)
        {
            writer(graph, FileMode.Append);
        }

        public List<object> readTruncate()
        {

            List<object> list = new List<object>();
            if (file == null)
                return list;

            for (int i = 0; i < attempts; i++) try
                {
                    if (File.Exists(file))
                    {
                        using (FileStream fStream
                            = File.Open(file,
                            FileMode.Open,
                            FileAccess.ReadWrite,
                            FileShare.None))
                        {
                            StreamReader sr = new StreamReader(fStream);
                            string data = sr.ReadToEnd();
                            string[] records = data.Split(GraphDelimiter);

                            foreach (string json in records)
                            {
                                object graph = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                if (graph != null)
                                    list.Add(graph);
                            }

                            // now truncate the file
                            fStream.SetLength(0);
                            fStream.Close(); // This flushes the content
                        }
                        break;
                    }
                    else
                    {
                        Thread.Sleep(timeout);
                    }
                }
                catch (IOException ioe)
                {
                    Thread.Sleep(timeout);
                }

            return list;
        }
    }
}

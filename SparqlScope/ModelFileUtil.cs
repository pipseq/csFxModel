using System;
using System.Collections.Generic;
using System.Threading;
using com.hp.hpl.jena.rdf.model;
using java.io;

namespace SparqlScope
{
    class ModelFileUtil
    {
        #region utils
        public static string getModelLang(string filename)
        {
            if (filename.ToLower().EndsWith(".owl")) return "RDF/XML";
            if (filename.ToLower().EndsWith(".xml")) return "RDF/XML";
            if (filename.ToLower().EndsWith(".n3")) return "N3";
            if (filename.ToLower().EndsWith(".ttl")) return "TTL";
            return "RDF/XML";
        }

        public static Model read(string file)
        {
            Model m = ModelFactory.createDefaultModel();
            FileInputStream fis = new FileInputStream(file);
            try
            {
                m.read(fis, null, getModelLang(file));
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
            fis.close();
            return m;
        }

        public static void write(Model m, string file)
        {
            FileOutputStream fis = new FileOutputStream(file);
            try
            {
                m.write(fis, getModelLang(file));
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
            fis.close();
        }
        #endregion
        #region util
        /*
        Originally part of SemExpertBase.
        This is experimental.  
        Write out indicator and price data to a journal for use in testing.
        NOTE: it works ok for a single pair, but has trouble with multiple pairs.
        */
        public static bool writeEnabled = true;
        public static System.IO.StreamWriter testSW = new System.IO.StreamWriter("stream.txt");
        private static List<String> writeQueue = new List<string>();
        private static Thread writeThread;
        private static bool keepWriteThread = true;
        protected static void writeData(string data)
        {
            if (writeEnabled)
                lock (writeQueue)
                {
                    writeQueue.Add(data);
                    if (writeThread == null)
                    {
                        writeThread = new Thread(new ThreadStart(writeQueuedData));
                        writeThread.Start();
                    }
                }
        }
        private static void writeQueuedData()
        {
            while (keepWriteThread)
            {
                while (writeQueue.Count > 0)
                {
                    string data = null;
                    lock (writeQueue)
                    {
                        data = writeQueue[0];
                        writeQueue.RemoveAt(0);
                    }
                    testSW.WriteLine(data);
                    testSW.Flush();
                }
                Thread.Sleep(200);
            }
        }
        #endregion

    }
}

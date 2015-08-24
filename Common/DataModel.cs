using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class DataModel
    {
        Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>> archive
        = new Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>>();
        private FileUtil fileUtil = new FileUtil();
        Dictionary<string, DateTime> mapTime = new Dictionary<string, DateTime>();
        IErrorHandler errorHandler;

        public DataModel(IErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        private void handleException(Exception e)
        {
            errorHandler.handleException(e);
        }

        #region saving and loading

        public void save(string file, bool std, List<string> coreData)
        {
            fileUtil.save(archive, file, std, coreData);
        }

        public void putArchive(Dictionary<string, Dictionary<string, object>> maps)
        {
            archive.Add(DateTime.Now, maps);
        }
        public List<string> load(string file)
        {

            List<string> ldts = new List<string>();
            try
            {
                archive = fileUtil.load(file);
            }
            catch (Exception e)
            {
                handleException(e);
            }
            List<DateTime> ldt = new List<DateTime>(archive.Keys);
            if (ldt.Count > 0)
            {
                mapTime = new Dictionary<string, DateTime>();
                ldt.Sort();
                foreach (DateTime dt in ldt)
                {
                    string dts = partDateTime(dt);
                    if (!mapTime.ContainsKey(dts))
                    {
                        mapTime.Add(dts, dt);
                        ldts.Add(dts);
                    }
                }
            }
            return ldts;
        }

        #endregion
        public ICollection<DateTime> getKeys()
        {
            return new List<DateTime>(archive.Keys);
        }

        public Dictionary<string, Dictionary<string, object>> get(DateTime key)
        {
            return archive[key];
        }

        public Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>> getArchive()
        {
            return archive;
        }

        public DateTime getMapTime(string key)
        {
            return mapTime[key];
        }


        public string partDateTime(DateTime dt)
        {
            string dts = dt.ToString();
            dts = dts.Substring(dts.IndexOf(' ') + 1);
            return dts;
        }

    }

    public interface IErrorHandler
    {
        void handleException(Exception e);
    }
}

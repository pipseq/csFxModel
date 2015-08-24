using System;
using System.Collections.Generic;
using System.Text;

namespace fxCoreLink
{
    public class CtrlTimer
    {
        private CtrlTimer() { }
        private static CtrlTimer instance = new CtrlTimer();
        public static CtrlTimer getInstance()
        {
            return instance;
        }
        private Dictionary<string, Dictionary<string, object>> map = new Dictionary<string, Dictionary<string, object>>();
        public void startTimer(string name)
        {
            if (!map.ContainsKey(name))
            {
                map.Add(name, new Dictionary<string, object>());
            }
            Dictionary<string, object> m = map[name];
            if (!m.ContainsKey("start"))
                m.Add("start", DateTime.Now);
            else
                m["start"] = DateTime.Now;
        }

        public void stopTimer(string name)
        {
            DateTime now = DateTime.Now;
            if (!map.ContainsKey(name))
            {
                startTimer(name);
            }
            Dictionary<string, object> m = map[name];
            DateTime start = (DateTime)m["start"];
            if (!m.ContainsKey("total"))
            {
                m.Add("total", new TimeSpan());
                m.Add("max", new TimeSpan());
                m.Add("min", new TimeSpan());
                m.Add("count", 0);
            }
            TimeSpan total = (TimeSpan)m["total"];
            TimeSpan max = (TimeSpan)m["max"];
            TimeSpan min = (TimeSpan)m["min"];
            int count = (int)m["count"];
            count++;
            total += now - start;
            if (max < now - start)
                m["max"] = now - start;
            if (min > now - start)
                m["min"] = now - start;
            m["total"] = total;
            m["count"] = count;
        }

        public override string ToString()
        {
            return ToString("");
        }
        public string ToString(string filter)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string name in map.Keys)
            {
                if (filter != ""
                    && !name.Contains(filter))
                    continue;
                sb.Append(name + ":");
                Dictionary<string, object> m = map[name];
                int cnt = 0;
                TimeSpan total = new TimeSpan();
                foreach (string stat in m.Keys)
                {
                    sb.Append(stat + "=" + m[stat] + " ");
                    if (stat == "count")
                        cnt = (int)m[stat];
                    if (stat == "total")
                        total = (TimeSpan)m[stat];
                }
                if (cnt > 0)
                    sb.AppendLine("average=" + (total.Milliseconds / cnt) + " ms");
            }
            return sb.ToString();
        }

        #region utility methods
        public static DateTime getTimeNow()
        {
            return DateTime.Now;
        }

        public static string getTimeNowFormatted()
        {
            return getTimeNow().ToString("yyyy-MM-dd HH:mm:ss.fff");
        }


        #endregion

    }
}

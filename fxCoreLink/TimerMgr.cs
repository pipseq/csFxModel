using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace fxCoreLink
{
    public class TimerMgr
    {
        Logger log = Logger.LogManager("TimerMgr");
        private TimerMgr()
        {
        }
        private static TimerMgr instance = new TimerMgr();

        public static TimerMgr getInstance()
        {
            return instance;
        }

        private List<Timer> list = new List<Timer>();
        private Dictionary<Timer, Dictionary<string, object>> TagMap = new Dictionary<Timer, Dictionary<string, object>>();
        private int identiferCounter = 1;

        public Timer create(string type, int interval)
        {
            Timer timer = new Timer();
            timer.Interval = interval * 1000; // as milliseconds
            list.Add(timer);
            Dictionary<string, object> map = new Dictionary<string, object>();
            TagMap.Add(timer,map);
            map.Add("type", type);
            map.Add("id", "" + identiferCounter++); // as a string
            log.debug("create, type=" + type + ", interval=" + timer.Interval + " ms");
            //updateDisplay();
            return timer;
        }

        public bool remove(Timer timer)
        {
            if (list.Contains(timer))
            {
                log.debug("remove, type="
                    + getTimerMap(timer, "type")
                    + ", interval=" + timer.Interval + " ms");
                log.debug("remove, map={" + getMapToString(timer) + "}");
                list.Remove(timer);
                TagMap.Remove(timer);
                timer.Stop();
                //updateDisplay();
                return true;
            }
            return false;
        }

        public bool removeById(string id)
        {
            Timer timer = getTimerById(id);
            return remove(timer);
        }

        private string getMapToString(Timer timer)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, object> map = TagMap[timer];
            foreach (string key in map.Keys)
            {
                sb.AppendFormat("{0}={1} ", key, map[key]);
            }
            return sb.ToString();
        }

        public bool stop(Timer timer)
        {
            timer.Stop();
            return remove(timer);
        }

        public object getTimerMap(Timer timer, string key)
        {
            return getTimerMap(timer, key, null);
        }

        public object getTimerMap(Timer timer, string key, object defaultValue)
        {
            Dictionary<string, object> map = TagMap[timer];
            if (map.ContainsKey(key))
                return map[key];
            else
                return defaultValue;
        }

        public void putTimerMap(Timer timer, string key, object value)
        {
            Dictionary<string, object> map = TagMap[timer];
            if (map.ContainsKey(key))
            {
                map[key] = value;
            }
            else
            {
                map.Add(key, value);
            }
        }
        public void removeTimerMap(Timer timer, string key)
        {
            Dictionary<string, object> map = TagMap[timer];
            if (map.ContainsKey(key))
            {
                map.Remove(key);
            }
        }

        // scheduledEvent, ordersArmed
        public string toString(Timer timer)
        {
            string id = (string)getTimerMap(timer, "id");
            string type = (string)getTimerMap(timer, "type");
            string pairs = (string)getTimerMap(timer, "pairs", "-");
            int iterations = (int)getTimerMap(timer, "iterations", 1);
            DateTime scheduledEvent = (DateTime)getTimerMap(timer, "startTime", DateTime.Now);
            //bool ordersArmed = (bool)getTimerMap(timer, "ordersArmed",false);
            //int interval = timer.Interval / 1000;
            return string.Format("{0} {1} {2} {3}",
                id,
                type,
                scheduledEvent.ToString("MM/dd@HH:mm"),
                pairs
                );
        }

        public Timer getTimerById(string id)
        {
            List<Timer> list = getTimerWithKeyValue("id", id);
            if (list.Count == 1)
                return list[0];
            return null;
        }
        // gets all
        public List<Timer> getTimerWithKeyValue(string key, object value)
        {
            List<Timer> lt = new List<Timer>();
            foreach (Timer t in list)
            {
                object v = getTimerMap(t, key);
                if (v != null && v.Equals(value))
                    lt.Add(t);
            }
            return lt;
        }
    }

}

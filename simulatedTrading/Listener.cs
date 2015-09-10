using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulatedTrading
{
    public class ListenerMgr
    {
        public List<Listener> list = new List<Listener>();

        public ListenerMgr() { }

        public void addListener(Listener listener)
        {
            list.Add(listener);
        }

        public bool removeListener(Listener listener)
        {
            return list.Remove(listener);
        }

        public List<Listener> getListeners()
        {
            return list;
        }
    }
}

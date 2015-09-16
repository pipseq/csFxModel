using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.fx;

namespace simulatedTrading
{
    public class TransactionManager : OrderListener, PositionListener, ClosedTradeListener
    {
        private static Logger log = Logger.LogManager("TransactionManager");

        private ClosedTrade closedTrade = new ClosedTrade();
        private Position position;
        private Order order;
        private IFxUpdates fxUpdates;
        private static TransactionManager instance = new TransactionManager();

        public IFxUpdates FxUpdates
        {
            get
            {
                return fxUpdates;
            }

            set
            {
                fxUpdates = value;
            }
        }

        // public to support unit tests, but should be private for singleton
        public TransactionManager()
        {
            position = new Position(closedTrade, this);
            order = new Order(closedTrade, position);
        }

        public static TransactionManager getInstance()
        {
            return instance;
        }

        public static double getPoiintSize(string pair)
        {
            if (pair.Contains("JPY"))
                return 0.01;
            return 0.0001;
        }

        public void priceUpdate(string pair, DateTime dt, double price)
        {
            getOrder().processOrders(pair, dt, price, price); // for testing, no spread
        }

        public void priceUpdate(string pair, DateTime dt, double bid, double ask)
        {
            getOrder().processOrders(pair, dt, bid, ask);
        }

        public Order getOrder()
        {
            return order;
        }

        public Position getPosition()
        {
            return position;
        }

        public ClosedTrade getClosedTrade()
        {
            return closedTrade;
        }
            
        public void orderChangeNotification(string pair, Dictionary<string, object> map, StateEvent e)
        {
            throw new NotImplementedException();
        }

        public void positionChangeNotification(string pair, Dictionary<string, object> map, StateEvent e)
        {
            if (e == StateEvent.Delete)
            {
                getOrder().closeOrders(pair, (string)map["entry"]);
            }
            else if (e == StateEvent.Create)
            {
            }
        }

        public void closedTradeChangeNotification(string pair, Dictionary<string, object> map, StateEvent e)
        {
            throw new NotImplementedException();
        }

        #region util

        public static void snapshot()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\n\t{0}\r\n", "Positions");
            printMap(getInstance().getPosition().mapPosition, sb);

            sb.AppendFormat("\r\n\t{0}\r\n", "Orders");
            printMap(getInstance().getOrder().mapEntryOrder, sb);

            sb.AppendFormat("\r\n\t{0}\r\n", "ClosedPositions");
            printMap(getInstance().getClosedTrade().mapClosedPosition, sb);

            Console.WriteLine(sb.ToString());
            log.debug(sb.ToString());
        }

        public static void printMap(Dictionary<string, List<Dictionary<string, object>>> map, StringBuilder sb)
        {
            foreach (string pair in map.Keys)
            {
                List<Dictionary<string, object>> list = map[pair];
                sb.AppendFormat("{0}\r\n", pair);

                foreach (Dictionary<string, object> m in list)
                {
                    foreach (string key in m.Keys)
                    {
                        object value = m[key];
                        sb.AppendFormat("\t{0}\t{1}\r\n", key,value);
                    }
                }
            }
        }

        #endregion
    }

    public enum StateEvent {
        Create = 1,
        Delete = -1,
        Change = 2
    }
    
    public interface Listener { }
    public interface OrderListener : Listener
    {
        void orderChangeNotification(string pair, Dictionary<string, object> map, StateEvent e);
    }

    public interface PositionListener : Listener
    {   
        void positionChangeNotification(string pair, Dictionary<string, object> map, StateEvent e);
    }

    public interface ClosedTradeListener : Listener
    {
        void closedTradeChangeNotification(string pair, Dictionary<string, object> map, StateEvent e);
    }
}

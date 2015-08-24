using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Configuration;

namespace Common
{
    public class Contract : BaseContract, Common.IContract
    {
        private static Dictionary<string, Dictionary<string, string>> map = new Dictionary<string, Dictionary<string, string>>();

        public Contract()
        {
            loadXml();
        }
        private static int cnt = 1;
        public static void loadXml()
        {
            XmlDocument doc = new XmlDocument();
            string contractDetailFile = ConfigurationManager.AppSettings["contractDetailFile"];
            doc.Load(contractDetailFile);
            map = new Dictionary<string, Dictionary<string, string>>();

            XmlNodeList nl = doc.SelectNodes("/contractDetails/*");
            foreach (XmlNode n in nl)
            {
                string cont = n.Name;
                Dictionary<string, string> detMap = new Dictionary<string, string>();
                map.Add(cont, detMap);
                string xp = string.Format("/contractDetails/{0}/*", cont);
                XmlNodeList dnl = doc.SelectNodes(xp);
                foreach (XmlNode dn in dnl)
                {
                    XmlAttribute att = dn.Attributes["name"];
                    string prop = att.Value;
                    string val = dn.InnerText;
                    detMap.Add(prop, val);
                }
                cnt++;
            }
        }
        private static string[] parseContract(string contract)
        {
            char[] ca = contract.ToCharArray();
            int i = ca.Length;

            string yr = new string(new char[] { ca[i - 2], ca[i - 1] });
            string mn = new string(new char[] { ca[i - 3] });
            char[] ca2 = new char[i - 3];
            for (int j = 0; j < i - 3; j++)
            {
                ca2[j] = ca[j];
            }
            string sm = new string(ca2);
            return new string[] { sm, mn, yr };
        }
        public static string getContractSymbol(string contract)
        {
            return parseContract(contract)[0];
        }
        public static string getContractMonth(string contract)
        {
            return parseContract(contract)[1];
        }
        public static string getContractYear(string contract)
        {
            return parseContract(contract)[2];
        }
        /**
         */
        public static Dictionary<string, string> getMap(string c)
        {
            string sym = getContractSymbol(c).ToLower();
            if (map.ContainsKey(sym))
                return map[sym];

            return null;
        }

        public override double getFactor(string contract)
        {
            Dictionary<string, string> map = getMap(contract);
            string s = map["Tick"];
            string[] sa = Regex.Split(s, "[= \\$]+");
            if (sa.Length >= 3) return double.NaN;  // country currency code
            try
            {
                double n = double.Parse(sa[0]);
                double d = double.Parse(sa[1]);
                return n / d;
            }
            catch (Exception)
            {
                return double.NaN;
            }
        }

        public override double getTick(string contract)
        {
            Dictionary<string, string> map = getMap(contract);
            string s = map["Tick"];
            string[] sa = Regex.Split(s, "[= \\$]+");
            if (sa.Length >= 3) return double.NaN;  // country currency code
            try
            {
                double n = double.Parse(sa[0]);
                return n;
            }
            catch (Exception)
            {
                return double.NaN;
            }
        }

        public double getMargin(string contract)
        {
            Dictionary<string, string> map = getMap(contract);
            string s = map["Initial Margin"];
            Match m = Regex.Match(s, "\\$([0-9\\,]+)");
            if (m.Groups.Count != 2) return double.NaN;
            Group g = m.Groups[1];
            {
                try
                {
                    double n = double.Parse(g.Value);
                    return n;
                }
                catch (Exception)
                {
                }
            }
            return double.NaN;
        }

    }
    public abstract class BaseContract
    {
        public abstract double getFactor(string contract);
        public abstract double getTick(string contract);

        public double calculateStopOrLimit(string contract, double last, double value, string orderType, bool stop)
        {
            double factor = getFactor(contract);
            double stopPrice = double.NaN;
            double limitPrice = double.NaN;
            double lastValue = last / factor;
            double stopValue = double.NaN;
            double limitValue = double.NaN;

            if (orderType == "STC" || orderType == "BTO")
            {
                stopValue = lastValue - value;
                limitValue = lastValue + value;
            }
            else if (orderType == "BTC" || orderType == "STO")
            {
                stopValue = lastValue + value;
                limitValue = lastValue - value;
            }
            stopPrice = stopValue * factor;
            limitPrice = limitValue * factor;

            if (stop) return Math.Round(stopPrice, 4);
            else return Math.Round(limitPrice, 4);
        }

        // alternate version returning whole number ticks as "points"
        //public double calculateTrailStop(string contract, double last, double value, string orderType)
        //{
        //    double stopPrice = calculateStopOrLimit(contract, last, value, orderType, true);
        //    double diff = Math.Abs(last - stopPrice);
        //    double tick = getTick(contract);
        //    double pts = Math.Round(diff,4) / tick;
        //    return Math.Ceiling(pts);
        //}

        // returns price difference points
        public double calculateTrailStop(string contract, double last, double value, string orderType)
        {
            double stopPrice = calculateStopOrLimit(contract, last, value, orderType, true);
            double diff = Math.Abs(last - stopPrice);
            return Math.Round(diff, 4);
        }

    }
    public interface IContract
    {
        double calculateStopOrLimit(string contract, double last, double value, string orderType, bool stop);
        double calculateTrailStop(string contract, double last, double value, string orderType);
        double getMargin(string contract);
        double getFactor(string contract);
        double getTick(string contract);
    }
}

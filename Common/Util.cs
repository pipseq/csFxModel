using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Common
{
    public class Util
    {
        #region security
        public static string decrypt(string epw)
        {
            string pw = decode(epw);
            return pw;
        }

        public static string decode(string data)
        {
            string result = "";
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                result = new String(decoded_char);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error in decode" + e.Message);
                //log.error("Error in decode" + e.Message);
            }
            return result;
        }
        #endregion

        #region parsing
        private static DateTime parseTime(string s)
        {
            DateTime dt = new DateTime();
            try
            {
                dt = DateTime.Parse(s);
            }
            catch (Exception)
            {
                //throw new Exception(string.Format(
                //    "Data {0} not DateTime parsed. ", s));
            }
            return dt;
        }
        private static double parseDouble(string s)
        {
            double result = 0.0;
            try
            {
                result = double.Parse(s);
            }
            catch (Exception)
            {
                //throw new Exception(string.Format(
                //    "Data {0} not double parsed. ", s));
            }
            return result;
        }
        #endregion

        #region date checks
        private static List<int[]> holidays;
        // good to 2014!!
        protected void Date_Load()
        {
            holidays = new List<int[]>();

            holidays.Add(new int[] { 1, 1, 1 }); // "New Years Day";
            holidays.Add(new int[] { 2, 1, 1 }); // "New Years Day";
            holidays.Add(new int[] { 3, 1, 1 }); // "New Years Day";
            holidays.Add(new int[] { 4, 1, 1 }); // "New Years Day";

            holidays.Add(new int[] { 1, 1, 17 }); // "Martin Luther King Day";
            holidays.Add(new int[] { 2, 1, 16 }); // "Martin Luther King Day";
            holidays.Add(new int[] { 3, 1, 21 }); // "Martin Luther King Day";
            holidays.Add(new int[] { 4, 1, 20 }); // "Martin Luther King Day";

            holidays.Add(new int[] { 1, 2, 21 }); // "Presidents' Day";
            holidays.Add(new int[] { 2, 2, 20 }); // "Presidents' Day";
            holidays.Add(new int[] { 3, 2, 18 }); // "Presidents' Day";
            holidays.Add(new int[] { 4, 2, 17 }); // "Presidents' Day";

            holidays.Add(new int[] { 1, 5, 30 }); // "Memorial Day";
            holidays.Add(new int[] { 2, 5, 28 }); // "Memorial Day";
            holidays.Add(new int[] { 3, 5, 27 }); // "Memorial Day";
            holidays.Add(new int[] { 4, 5, 26 }); // "Memorial Day";

            holidays.Add(new int[] { 1, 7, 4 }); // "Independence Day";
            holidays.Add(new int[] { 2, 7, 4 }); // "Independence Day";
            holidays.Add(new int[] { 3, 7, 4 }); // "Independence Day";
            holidays.Add(new int[] { 4, 7, 4 }); // "Independence Day";

            holidays.Add(new int[] { 1, 9, 5 }); // "Labor day";
            holidays.Add(new int[] { 2, 9, 3 }); // "Labor day";
            holidays.Add(new int[] { 3, 9, 2 }); // "Labor day";
            holidays.Add(new int[] { 4, 9, 1 }); // "Labor day";

            holidays.Add(new int[] { 1, 10, 10 }); // "Columbus Day";
            holidays.Add(new int[] { 2, 10, 8 }); // "Columbus Day";
            holidays.Add(new int[] { 3, 10, 14 }); // "Columbus Day";
            holidays.Add(new int[] { 4, 10, 13 }); // "Columbus Day";

            holidays.Add(new int[] { 1, 11, 11 }); // "Veterans Day";
            holidays.Add(new int[] { 2, 11, 12 }); // "Veterans Day";
            holidays.Add(new int[] { 3, 11, 11 }); // "Veterans Day";
            holidays.Add(new int[] { 4, 11, 11 }); // "Veterans Day";

            holidays.Add(new int[] { 1, 11, 24 }); // "Thanksgiving Day";
            holidays.Add(new int[] { 2, 11, 22 }); // "Thanksgiving Day";
            holidays.Add(new int[] { 3, 11, 28 }); // "Thanksgiving Day";
            holidays.Add(new int[] { 4, 11, 27 }); // "Thanksgiving Day";

            holidays.Add(new int[] { 1, 12, 25 }); // "Christmas Day";
            holidays.Add(new int[] { 2, 12, 25 }); // "Christmas Day";
            holidays.Add(new int[] { 3, 12, 25 }); // "Christmas Day";
            holidays.Add(new int[] { 4, 12, 25 }); // "Christmas Day";
        }

        public bool isBetweenInclusive(string t1, string t2)
        {
            TimeSpan ts1 = TimeSpan.Parse(t1);
            TimeSpan ts2 = TimeSpan.Parse(t2);
            return isBetweenInclusive(ts1, ts2);
        }
        public bool isBetweenInclusive(TimeSpan t1, TimeSpan t2)
        {
            bool b1 = DateTime.Now >= DateTime.Today.Add(t1);
            bool b2 = DateTime.Now <= DateTime.Today.Add(t2);
            return b1 && b2;
        }
        public bool isNearClosed()
        {
            return isNearClosed("16:00:00");
        }

        public bool isNearClosed(string sBeginTime)
        {
            bool b1 = isClosed(DateTime.Now);

            bool b2 = isBetweenInclusive(sBeginTime, "17:00:00"); // 4pm - 5pm
            return b1 || b2;
        }

        public bool isClosed()
        {
            return isClosed(DateTime.Now);
        }

        public bool isClosed(string dateStr, string timespanStr)
        {

            DateTime date = DateTime.Parse(dateStr);
            TimeSpan time = TimeSpan.Parse(timespanStr);

            DateTime dt = date.Add(time);
            return isClosed(dt);
        }

        public bool isClosed(string dateStr)
        {

            DateTime date = DateTime.Parse(dateStr);
            return isClosed(date);
        }

        private Comparer comp = new Comparer();

        public bool isClosed(DateTime dt)
        {
            if (holidays == null) Date_Load();
            int day = dt.Day;
            int mn = dt.Month;
            int yr = dt.Year;
            int hr = dt.Hour;
            return
                dt.DayOfWeek == DayOfWeek.Monday && (hr >= 17 && hr < 18) ||
                dt.DayOfWeek == DayOfWeek.Tuesday && (hr >= 17 && hr < 18) ||
                dt.DayOfWeek == DayOfWeek.Wednesday && (hr >= 17 && hr < 18) ||
                dt.DayOfWeek == DayOfWeek.Thursday && (hr >= 17 && hr < 18) ||
                dt.DayOfWeek == DayOfWeek.Saturday ||
                dt.DayOfWeek == DayOfWeek.Friday && hr >= 17 ||
                dt.DayOfWeek == DayOfWeek.Sunday && hr < 18 ||
                holidays.Contains(new int[] { yr - 2010, mn, day },
                comp
                ) && hr < 18 ||
                holidays.Contains(new int[] { yr - 2010, mn, day - 1 },
                comp
                ) && hr >= 17
                ;
        }

        #endregion
        #region time utility
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
    #region special classes
    class Comparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i]) return false;
            }
            return true;
        }

        public int GetHashCode(int[] x)
        {
            int hc = 0;
            for (int i = 0; i < x.Length; i++)
            {
                hc += x[i].GetHashCode();
            }
            return hc;
        }
    }
#endregion
}

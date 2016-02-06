using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Clock
    {
        private static IClock source;

        public static IClock Source
        {
            get
            {
                return source;
            }

            set
            {
                source = value;
            }
        }

        public static DateTime Now()
        {
            if (Source == null)
                return DateTime.Now;
            else
                return Source.Now();
        }
    }
}

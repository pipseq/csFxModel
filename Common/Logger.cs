using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.IO;

namespace Common
{
    public interface Output
    {
        void appendLine(string s);
    }

    public class Logger
    {
        private static readonly int ErrorLevel = 4;
        private static readonly int WarnLevel = 3;
        private static readonly int InfoLevel = 2;
        private static readonly int DebugLevel = 1;
        public enum LogLevel { ErrorLevel, WarnLevel, InfoLevel, DebugLevel };
        private static string fname = "OcrExample.log";
        private static string format = "{0}:{1}:{2}:{3}";
        private static string dateTimeFormat = "MM/dd/yy HH:mm:ss.fff";
        private string clsname;
        public int currentLevel = DebugLevel;
        private static Output console;
        private Logger(string clsname)
        {
            this.clsname = clsname;
            fname = ConfigurationManager.AppSettings["logFname"];
            if (fname == null)
            {
                    fname = "default.log";
            }
            string projectFolder = ConfigurationManager.AppSettings["projectFolder"];
            string logFolder = ConfigurationManager.AppSettings["logFolder"];
            if (projectFolder != null)
            {
                fname = projectFolder + @"\" + logFolder + @"\" + fname;
            }
        }

        public static Logger LogManager(Type cls)
        {
            return new Logger(cls.Name);
        }
        public static Logger LogManager(string name)
        {
            return new Logger(name);
        }
        public static void SetConsole(Output output)
        {
            console = output;
        }
        private void getLogLevel()
        {
            string levelStr = ConfigurationManager.AppSettings["logLevel"];
            if (levelStr != null)
            {
                if (levelStr.Equals("error", StringComparison.OrdinalIgnoreCase))
                    currentLevel = ErrorLevel;
                else if (levelStr.Equals("warn", StringComparison.OrdinalIgnoreCase))
                    currentLevel = WarnLevel;
                else if (levelStr.Equals("info", StringComparison.OrdinalIgnoreCase))
                    currentLevel = InfoLevel;
                else if (levelStr.Equals("debug", StringComparison.OrdinalIgnoreCase))
                    currentLevel = DebugLevel;
            }
        }
        private bool checkLevel(int level)
        {
            getLogLevel();
            return level >= currentLevel;
        }
        public void info(string s)
        {
            if (checkLevel(InfoLevel))
                output(clsname, "INFO", s);
        }
        public void info(string s, params object[] args)
        {
            info(string.Format(s,args));
        }
        public void warn(string s)
        {
            if (checkLevel(WarnLevel))
                output(clsname, "WARN", s);
        }
        public void warn(string s, params object[] args)
        {
            warn(string.Format(s, args));
        }
        public void debug(string s)
        {
            if (checkLevel(DebugLevel))
                output(clsname, "DEBUG", s);
        }
        public void debug(string s, params object[] args)
        {
            debug(string.Format(s, args));
        }
        public void error(string s)
        {
            if (checkLevel(ErrorLevel))
                output(clsname, "ERROR", s);
        }
        public void error(string s, params object[] args)
        {
            error(string.Format(s, args));
        }
        private void output(string clsname, string level, string s)
        {
            output(string.Format(format, DateTime.Now.ToString(dateTimeFormat), clsname, level, s));
            if (console != null)
                console.appendLine(string.Format("{0}",s));
        }
        private void output(string s)
        {
            lock (fname)
            {
                System.Console.WriteLine(s);
                StreamWriter sw = new StreamWriter(fname, true);
                if (sw != null)
                {
                    sw.WriteLine(s);
                    sw.Flush();
                }
                sw.Close();
            }
        }
    }
}

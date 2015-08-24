using Common;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;

namespace fxCoreLink
{
    public class ExpertScript : ExpertBase, IExpert
    {

        private Logger log = Logger.LogManager("ExpertScript");
        private ScriptEngine m_engine;
        private ScriptScope m_scope = null;
        private string filename;
        private dynamic root;

        public Logger Log
        {
            get
            {
                return log;
            }

            set
            {
                log = value;
            }
        }

        public ExpertScript(string filename, string pair, TimeFrame timeFrame)
            : base(pair, timeFrame)
        {
            this.filename = filename;
            Log.info("filename=" + filename + ", pair=" + pair + ", timeFrame=" + ExpertFactory.timeFrameMap[timeFrame]);
            m_engine = Python.CreateEngine();
            m_scope = m_engine.CreateScope();
        }

        public ExpertScript(string filename, string pair, TimeFrame timeFrame, bool debug)
            : base(pair, timeFrame)
        {
            this.filename = filename;
            Log.info("debug=" + debug + ", filename=" + filename + ", pair=" + pair + ", timeFrame=" + ExpertFactory.timeFrameMap[timeFrame]);
            var options = new Dictionary<string, object>();
            options["Debug"] = debug 
                ? ScriptingRuntimeHelpers.True
                : ScriptingRuntimeHelpers.False;
            m_engine = Python.CreateEngine(options);
            m_scope = m_engine.CreateScope();
        }

        public override void start()
        {
            try
            {
                m_scope.SetVariable("Pair", Pair);
                m_scope.SetVariable("TimeFrame", this.Timeframe);
                m_scope.SetVariable("PriceComponent", PriceComponent.BidClose);
                m_scope.SetVariable("Factory", Factory);
                m_scope.SetVariable("Base", this);

                root = m_engine.ExecuteFile(filename, m_scope);
                root.Start();
            }
            catch (Exception ex)
            {
                Factory.Display.appendLine(ex.Message);
                Log.error("start(), filename=" + filename + ", pair=" + Pair + ", Exception=" + ex.Message);
            }
        }

        public override void stop()
        {
            try
            {
                root.Stop();
            }
            catch (Exception ex)
            {
                Factory.Display.appendLine(ex.Message);
                Log.error("stop(), filename=" + filename + ", pair=" + Pair + ", Exception=" + ex.Message);
            }
        }

        public override void timeUpdate(TimeFrame timeFrame)
        {
            try
            {
                root.timeUpdate(timeFrame);
            }
            catch (Exception ex)
            {
                Factory.Display.appendLine(ex.Message);
                Log.error("timeUpdate(), filename=" + filename + ", pair=" + Pair + ", Exception=" + ex.Message);
            }
        }

        public override void priceUpdate(DateTime datetime, double bid, double ask)
        {
            try
            {
                root.priceUpdate(datetime, bid, ask);
            }
            catch (Exception ex)
            {
                Factory.Display.appendLine(ex.Message);
                Log.error("priceUpdate(), filename=" + filename + ", pair=" + Pair + ", Exception=" + ex.Message);
            }
        }
    }
}

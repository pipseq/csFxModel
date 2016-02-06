using fxCoreLink;
using System;
using System.Collections.Generic;
using System.Threading;
using Common.fx;
using com.hp.hpl.jena.rdf.model;
using org.pipseq.rdf.jena.model;
using java.io;
using org.pipseq.spin;
using org.pipseq.rdf.jena.cfg;
using Newtonsoft.Json;
using org.pipseq.common.external;
using Common;

namespace csExperts
{
    public class SemanticExpertBase : ExpertBase
    {
        private static Common.Logger log = Common.Logger.LogManager("SemExpertBase");
        //protected Model model;
        protected static readonly String modelProlog = @"
@prefix fxs: <http://pipseq.org/2016/01/fx/strategy#> .
@prefix pip: <http://pipseq.org/2016/01/forex#> .
@prefix rdfs:    <http://www.w3.org/2000/01/rdf-schema#> .
@prefix owl:     <http://www.w3.org/2002/07/owl#> .
@prefix xsd:     <http://www.w3.org/2001/XMLSchema#> .
@prefix rdf:     <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .

";
        protected static readonly String queryProlog = @"
prefix pip:     <http://pipseq.org/2016/01/forex#> 
prefix fxs:     <http://pipseq.org/2016/01/fx/strategy#> 
prefix rdfs:    <http://www.w3.org/2000/01/rdf-schema#> 
prefix owl:     <http://www.w3.org/2002/07/owl#> 
prefix xsd:     <http://www.w3.org/2001/XMLSchema#> 
prefix rdf:     <http://www.w3.org/1999/02/22-rdf-syntax-ns#> 
prefix fn:		<http://www.w3.org/2005/xpath-functions#> 

";
        private static readonly String syphonQuery = @"
        describe ?s
        {
			?s a <http://pipseq.org/2016/01/forex#TradeRecommendation> .
		}
";

        static SemanticExpertBase() {
            WrapperRegistry.getInstance().setDefaultLoggingLevel(5);
        }


        private RuleEngine ruleEngine;
        protected Model model;
        private static bool ruleEngineQueueRun = true;
        private static List<SemanticExpertBase> queueRuleEngine = new List<SemanticExpertBase>();
        private bool stopLossActive = true;
        private int stopPips = 10;
        private bool limitActive = false;
        private int limitPips = 10;
        private int amount = 1000;
        private double lastBid = 0.0;
        private double lastAsk = 0.0;

        protected RuleEngine RuleEngine
        {
            get
            {
                return ruleEngine;
            }

            set
            {
                ruleEngine = value;
            }
        }

        public bool StopLossActive
        {
            get
            {
                return stopLossActive;
            }

            set
            {
                stopLossActive = value;
            }
        }

        public int StopPips
        {
            get
            {
                return stopPips;
            }

            set
            {
                stopPips = value;
            }
        }

        public bool LimitActive
        {
            get
            {
                return limitActive;
            }

            set
            {
                limitActive = value;
            }
        }

        public int LimitPips
        {
            get
            {
                return limitPips;
            }

            set
            {
                limitPips = value;
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }

            set
            {
                amount = value;
            }
        }

        public double LastBid
        {
            get
            {
                return lastBid;
            }

            set
            {
                lastBid = value;
            }
        }

        public double LastAsk
        {
            get
            {
                return lastAsk;
            }

            set
            {
                lastAsk = value;
            }
        }

        public SemanticExpertBase(string pair, TimeFrame timeframe, string[] modelFiles, bool diagnostics, int loglevel) : base(pair, timeframe)
        {
            RuleEngineFactory.getInstance().setModelFiles(modelFiles);
            WrapperRegistry.getInstance().setDefaultLoggingLevel(loglevel);

            RuleEngine = RuleEngineFactory.getInstance().getRuleEngine(pair);
            RuleEngine.setFeedbackQuery(@"
            describe ?s {
			?s a <http://pipseq.org/2016/01/forex#TradeRecommendation> .
		}
            ");
            RuleEngine.setOutcomeQuery(@"
        describe ?s {
			?s a <http://pipseq.org/2016/01/forex#Trade> .
		}
            ");
            RuleEngine.setDiagnostics(diagnostics);
        }

        private void setContext(TimeFrame timeframe)
        {
            Triple.remove(RuleEngine.getModel(), "pip:RuleContext"); // Change the context!!

            // set rule context
            string ruleContext = @"
                pip:RuleContext
                rdf:type pip:RuleContextSingleton;
                pip:hasInstrument pip:{0};
                pip:hasTimeFrame pip:{1};
                .
            ";
            string rc = string.Format(ruleContext, normalizePair(Pair), TimePriceComponents.timeFrameMap[timeframe]);
            RuleEngine.insertModel(modelProlog + rc);

        }

        protected string normalizePair(string pin)
        {
            if (pin.Length == 6) return pin;
            string[] pinFlds = pin.Split('/');

            if (pinFlds.Length == 2)
                return pinFlds[0] + pinFlds[1];
            throw new Exception("Unrecognized delimiter in pair: " + pin);
        }
        private static ExternalClock ec = new ExternalClock();

        public void timeUpdate(TimeFrame timeFrame, object signal)
        {
            string dts = Clock.Now().ToString("MM/dd/yyyy HH:mm:ss.fffzzz");
            ec.setTime(dts);
            setContext(timeFrame);

            run();

        }

        public IList<Dictionary<string, object>> queryListMap(Model model, string query)
        {
            string json = Sparql.queryJsonListMap(model, query);
            IList<Dictionary<string, object>> listMap = JsonConvert.DeserializeObject<IList<Dictionary<string, object>>>(json);
            return listMap;
        }

        public Dictionary<string, object> queryMap(Model model, string query)
        {
            string json = Sparql.queryJsonListMap(model, query);
            List<Dictionary<string, object>> lmap = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            Dictionary<string, object> map = new Dictionary<string, object>();
            foreach (Dictionary<string, object> map0 in lmap) {
                map.Add((string)map0["p"], map0["o"]);
            }
            return map;
        }

        // need to move to win thread via delegate
        public void scope()
        {
            scope(RuleEngine.getModel());
        }

        public void scope(Model model)
        {
            this.Factory.Display.scope(model);
        }


        protected object putResult(double result, object indicator, string type, string predicate, object suffix)
        {
            //               putResult(rsi, rInd, "pip:RSIResult", "pip:hasIndicatorResult", sequence);
            //object subj = Triple.createResource(ruleEngine.getModel(), type, suffix);
            string subj = "" + type + "_" + suffix;
            string ts = Factory.Now().ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
            string ttlTemplate = @"
    {0}
        rdf:type {1} ;
        pip:Value '{4}'^^xsd:float ;
        pip:TimeStamp '{5}'^^xsd:dateTime ;
    .
    {2} {3} {0} .
";
            string ttl = string.Format(ttlTemplate,
                subj,
                type,
                indicator,
                predicate,
                result,
                ts);

            RuleEngine.insertModel(modelProlog + ttl);

            return subj;
        }
        object lockobject = new object();
        void run()
        {
            if (Factory.Control.isJournalRead())
            {
                try
                {
                    System.Console.WriteLine("running RE: " + RuleEngine.getModelWrapper().getModelName());
                    ModelWrapper mw = RuleEngine.run();

                    if (RuleEngine.hasOutcomeResults())
                    {
                        trade(mw);
                    }

                }
                catch (Exception npe)
                {
                    log.error("RuleEngine " + RuleEngine.getModelWrapper().getModelName() + ":" + npe.Message);
                    System.Console.WriteLine("RuleEngine " + RuleEngine.getModelWrapper().getModelName()+":"+npe.Message);
                }
        }
            else // in realtime threads can't wait for ruleEngine?
            {
                queueRuleEngine.Add(this);
                if (ruleEngineThread == null)
                    //lock (lockobject)
                {
                    ruleEngineThread = new Thread(new ThreadStart(runRuleEngine));
                    ruleEngineThread.Start();
                }
            }
        }
        private static Thread ruleEngineThread;
        private static void runRuleEngine()
        {
            while (ruleEngineQueueRun)
            {
                while (queueRuleEngine.Count > 0)
                {
                    SemanticExpertBase seb = queueRuleEngine[0];
                    queueRuleEngine.RemoveAt(0);
                    try
                    {
                        System.Console.WriteLine("running RE: " + seb.RuleEngine.getModelWrapper().getModelName());
                        ModelWrapper mw = seb.RuleEngine.run();

                        if (seb.RuleEngine.hasOutcomeResults())
                        {
                            seb.trade(mw);
                        }
                    }
                    catch (NullReferenceException npe)
                    {
                        log.error("RuleEngine " + seb.RuleEngine.getModelWrapper().getModelName() + ":" + npe.Message);
                        System.Console.WriteLine("RuleEngine " + seb.RuleEngine.getModelWrapper().getModelName() + ":" + npe.Message);
                    }

                }
                Thread.Sleep(200);
            }
        }

        void trade(ModelWrapper mw)
        {
            log.debug("Trade!! for {0}", Pair);
            System.Console.WriteLine("Trade!!");
            // -d2cfd90:15164328ba1:-7a7c @rdf:type :Trade; 
            // -d2cfd90:15164328ba1:-7a7c @:TimeStamp "2015-12-02T14:36:59.535-05:00"^^xsd:dateTime; 
            // -d2cfd90:15164328ba1:-7a7c @:hasPosition :PositionLong
            string query = @"
                select ?p ?o {
                    ?s a pip:Trade .
                    ?s ?p ?o .
                    }
            ";

            string trade = "n/a";
            Dictionary<string, object> tradeMap = queryMap(mw.get(), query);

            if (tradeMap["rdf:type"].Equals("pip:Trade")
                && tradeMap["pip:hasPosition"].Equals("pip:PositionLong")
                )
            {
                Factory.Display.appendLine("Go Long {0}", Pair);
                System.Console.WriteLine("Go Long");
                trade = "long";
                    string entry = "BUY";
                    Factory.Display.appendLine(String.Format("Open, {0}, {1}, price={2}", entry, Pair, LastBid));
                    enterPosition(entry, Amount, LastBid, StopPips, false, LimitPips);
            }
            else if (tradeMap["rdf:type"].Equals("pip:Trade")
            && tradeMap["pip:hasPosition"].Equals("pip:PositionShort")
            )
            {
                Factory.Display.appendLine("Go Short {0}", Pair);
                System.Console.WriteLine("Go Short");
                trade = "short";
                string entry = "SELL";
                Factory.Display.appendLine(String.Format("Open, {0}, {1}, price={2}", entry, Pair, LastBid));
                enterPosition(entry, Amount, LastBid, StopPips, false, LimitPips);
            }

            log.debug("Trade results = {0} for {0}", trade , Pair);


        }

    }
}

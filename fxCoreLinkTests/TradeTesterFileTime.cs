using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.pipseq.spin;
using org.pipseq.rdf.jena.model;
using com.hp.hpl.jena.rdf.model;
using System.Windows.Forms;
using org.pipseq.rdf.jena.cfg;
using Newtonsoft.Json;
using org.pipseq.common.external;

namespace fxCoreLinkTests
{
    [TestClass]
    public class TradeTesterFilTimee : Form
    {
        static String prolog = @"
@prefix fxs: <http://pipseq.org/2016/01/fx/strategy#> .
@prefix pip: <http://pipseq.org/2016/01/forex#> .
@prefix rdfs:    <http://www.w3.org/2000/01/rdf-schema#> .
@prefix owl:     <http://www.w3.org/2002/07/owl#> .
@prefix xsd:     <http://www.w3.org/2001/XMLSchema#> .
@prefix rdf:     <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .
@prefix fn:		<http://www.w3.org/2005/xpath-functions#> .

";

        RuleEngine ruleEngine;
        int trades = 0;
        ExternalClock ec = new ExternalClock();

        [TestMethod]
        public void testRuleEngineWithFileTime()
        {
            for (int i = 0; i < 1; i++)
            {
                testRun();
            }
        }

        private string setTime(DateTime dt)
        {
            string dtios = dt.ToString("MM/dd/yyyyTHH:mm:ss.fffzzz");
            ec.setTime(dt.ToString("MM/dd/yyyy HH:mm:ss.fffzzz"));
            return dtios;
        }

        private void testRun()
        {

            WrapperRegistry.getInstance().setDefaultLoggingLevel(2);
            RuleEngineFactory.getInstance().setModelFiles(new string[]
                {
            "C:/work/semFxModel/var/models/pipseq.org/2016/01/forex.ttl",
            "C:/work/semFxModel/var/models/pipseq.org/2016/01/fx/strategy.ttl"
                });
            ruleEngine = RuleEngineFactory.getInstance().getRuleEngine("test");
            ruleEngine.setFeedbackQuery(@"
            describe ?s {
			?s a <http://pipseq.org/2016/01/forex#TradeRecommendation> .
		}
            ");
            ruleEngine.setOutcomeQuery(@"
        describe ?s {
			?s a <http://pipseq.org/2016/01/forex#Trade> .
		}
            ");
            ruleEngine.setDiagnostics(true);

            //string file = "..\\..\\stream.txt";
            string file = @"C:\work\semFxModel\fxModelDemo\bin\Debug\stream-7.txt";

            int i = 100;
            using (StreamReader sr = new StreamReader(file))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    i++;
                    string[] fields = line.Trim().Split('\t');
                    if (fields.Length < 5) return;
                    if (fields[0] == "EMA" && fields[2] == "m1")
                    {
                        m1EMA(i, fields);
                    }
                    if (fields[0] == "RSI" && fields[2] == "m5")
                    {
                        m5RSI(i, fields);
                    }
                    if (fields[0] == "EMA" && fields[2] == "m5")
                    {
                        m5EMA(i, fields);
                    }
                    if (ruleEngine.hasOutcomeResults())
                    {
                        System.Diagnostics.Trace.WriteLine("Trade!!!");
                    }
                }
            }

            Assert.AreEqual(trades, 84);
        }

        void m1EMA(int i, string[] fields)
        {
            double ask = double.Parse(fields[3]);
            double bid = double.Parse(fields[4]);
            double fast = double.Parse(fields[5]);
            double slow = double.Parse(fields[6]);
            DateTime dt = DateTime.Parse(fields[7]);

            Triple.remove(ruleEngine.getModel(), "pip:RuleContext"); // Change the context!!
            string s = @"

		pip:RuleContext
		rdf:type pip:RuleContextSingleton ;
		pip:hasInstrument pip:AUDJPY ;
		pip:hasTimeFrame pip:m1 ;
.

pip:EMAResult_f_{0}
  rdf:type pip:EMAResult ;
  pip:Value '{1}'^^xsd:float ;
  pip:TimeStamp '{3}' ;
.
pip:EMAResult_s_{0}
  rdf:type pip:EMAResult ;
  pip:Value '{2}'^^xsd:float ;
  pip:TimeStamp '{3}' ;
.
fxs:EMA_8_1 pip:hasIndicatorResult pip:EMAResult_f_{0}
.
fxs:EMA_34_1 pip:hasIndicatorResult pip:EMAResult_s_{0}
.
fxs:Ask_{0}
  rdf:type pip:Ask ;
  pip:Value '{4}'^^xsd:float ;
  pip:TimeStamp '{3}' ;
.
fxs:Bid_{0}
  rdf:type pip:Bid ;
  pip:Value '{5}'^^xsd:float ;
  pip:TimeStamp '{3}' ;
.
fxs:PriceFastSlowPattern_1
		pip:hasAsk	fxs:Ask_{0} ;
		pip:hasBid	fxs:Bid_{0} ;
		.
";

            string dtios = setTime(dt);
            string sf = string.Format(s, i, fast, slow, dtios, ask, bid);
            ruleEngine.insertModel(prolog + sf);
            ModelWrapper mw = ruleEngine.run();
            if (ruleEngine.hasOutcomeResults())
            {
                trades++;
            }

        }
        void m5EMA(int i, string[] fields)
        {
            double ask = double.Parse(fields[3]);
            double bid = double.Parse(fields[4]);
            double fast = double.Parse(fields[5]);
            double slow = double.Parse(fields[6]);
            DateTime dt = DateTime.Parse(fields[7]);

            Triple.remove(ruleEngine.getModel(), "pip:RuleContext"); // Change the context!!

            string s = @"

		pip:RuleContext
		rdf:type pip:RuleContextSingleton ;
		pip:hasInstrument pip:AUDJPY ;
		pip:hasTimeFrame pip:m5 ;
.

pip:EMAResult_f_{0}
  rdf:type pip:EMAResult ;
  pip:Value '{1}'^^xsd:float ;
  pip:TimeStamp '{3}' ;
.
pip:EMAResult_s_{0}
  rdf:type pip:EMAResult ;
  pip:Value '{2}'^^xsd:float ;
  pip:TimeStamp '{3}' ;
.
fxs:EMA_8 pip:hasIndicatorResult pip:EMAResult_f_{0}
.
fxs:EMA_34 pip:hasIndicatorResult pip:EMAResult_s_{0}
.
fxs:Ask_{0}
  rdf:type pip:Ask ;
  pip:Value '{4}'^^xsd:float ;
  pip:TimeStamp '{3}' ;
.
fxs:Bid_{0}
  rdf:type pip:Bid ;
  pip:Value '{5}'^^xsd:float ;
  pip:TimeStamp '{3}' ;
.
fxs:PriceFastSlowPattern_1
		pip:hasAsk	fxs:Ask_{0} ;
		pip:hasBid	fxs:Bid_{0} ;
		.
";
            string dtios = setTime(dt);
            string sf = string.Format(s, i, fast, slow, dtios, ask, bid);
            ruleEngine.insertModel(prolog + sf);
            ModelWrapper mw = ruleEngine.run();
            if (ruleEngine.hasOutcomeResults())
            {
                trades++;
            }
        }
        void m5RSI(int i, string[] fields)
        {
            double rsi = double.Parse(fields[3]);
            DateTime dt = DateTime.Parse(fields[4]);

            Triple.remove(ruleEngine.getModel(), "pip:RuleContext"); // Change the context!!
            string s = @"

		pip:RuleContext
		rdf:type pip:RuleContextSingleton ;
		pip:hasInstrument pip:AUDJPY ;
		pip:hasTimeFrame pip:m5 ;
.

pip:RSIResult_{0}
  rdf:type pip:RSIResult ;
  pip:Value '{1}'^^xsd:float ;
  pip:TimeStamp '{2}' ;
.
fxs:RSI_9 pip:hasIndicatorResult pip:RSIResult_{0} ;
.
";
            string dtios = setTime(dt);
            string sf = string.Format(s, i, rsi, dtios);
            ruleEngine.insertModel(prolog + sf);
            ModelWrapper mw = ruleEngine.run();
            if (ruleEngine.hasOutcomeResults())
            {
                trades++;
            }
        }

    }

}
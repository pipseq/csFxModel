using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.pipseq.spin;
using org.pipseq.rdf.jena.model;
using com.hp.hpl.jena.rdf.model;
using System.Windows.Forms;
using org.pipseq.rdf.jena.cfg;
using System.Collections.Generic;
using Newtonsoft.Json;
using org.pipseq.common.external;

namespace fxCoreLinkTests
{

    // Note extension of test class from Form to enable Sparql scope during tests
    [TestClass]
    public class TradeTesterTime : Form
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
        ExternalClock ec = new ExternalClock();
        string Pair = "AUDJPY";

        // This is a test of RuleEngine.
        // Three sets of data are processed with selected
        // inferences syphoned off to a cross-timeframe 
        // model.  A final execution produces inferences
        // for a trade signal
        [TestMethod]
        public void testRuleEngineTime()
        {
            setup();
            test1();
            setup();
            test2();
            setup();
            test3();
            setup();
            test4();
            setup();
            test5();
            testPairs();
        }

        void testPairs()
        {
            String[] pairs = {
"AUDJPY",
"AUDUSD",
"EURCHF",
"EURGBP",
"EURJPY",
"EURUSD",
"GBPJPY",
"GBPUSD",
"NZDUSD",
"USDCAD",
"USDCHF",
"USDCNH",
"USDJPY",
            };
            foreach (String p in pairs)
            {
                Pair = p;
                setup();
                test5();


            }
        }
            void setup()
        {
            RuleEngineFactory.reset();
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

            // Useful debugging technique for capturing java stdout
            // and printing to c# out (see Debug.WriteLine() below)
            //java.io.ByteArrayOutputStream baos = new java.io.ByteArrayOutputStream();
            //java.io.PrintStream ps = new java.io.PrintStream(baos);
            //java.lang.System.setOut(ps);

        }

        void test1()
        {
            ec.setTime("10/19/2014 14:20:55");
            emam5(false);
            emam1(false);

            ec.setTime("10/19/2014 14:30:55");
            rsim5(true);
        }

        void test2()
        {
            rsim5(false);
            emam1(false);
            emam5(true);
        }

        void test3()
        {
            rsim5(false);
            emam5(false);
            emam1(true);
        }

        void test4()
        {
            emam1(false);
            emam5(false);
            rsim5(true);
        }

        void test5()
        {
            emam1(false);
            rsim5(false);
            emam5(true);
        }

        void emam1(bool result)
        {
            Triple.remove(ruleEngine.getModel(), "pip:RuleContext"); // Change the context!!

            String s1 = @"

		pip:RuleContext
		rdf:type pip:RuleContextSingleton ;
		pip:hasInstrument pip:{0} ;
		pip:hasTimeFrame pip:m1 ;
.
pip:EMAResult_f102_m1
  rdf:type pip:EMAResult ;
  pip:Value '89.1'^^xsd:float ;
  pip:TimeStamp '2015-10-19T14:08:17.5Z' ;
.
pip:EMAResult_s102_m1
  rdf:type pip:EMAResult ;
  pip:Value '89.0'^^xsd:float ;
  pip:TimeStamp '2015-10-19T14:08:17.639Z' ;
.
fxs:EMA_8_1 pip:hasIndicatorResult pip:EMAResult_f102_m1
.
fxs:EMA_34_1 pip:hasIndicatorResult pip:EMAResult_s102_m1
.
pip:EMAResult_f103_m1
  rdf:type pip:EMAResult ;
  pip:Value '89.2'^^xsd:float ;
  pip:TimeStamp '2015-10-19T14:09:17.5Z' ;
.
pip:EMAResult_s103_m1
  rdf:type pip:EMAResult ;
  pip:Value '89.1'^^xsd:float ;
  pip:TimeStamp '2015-10-19T14:09:17.639Z' ;
.
fxs:EMA_8_1 pip:hasIndicatorResult pip:EMAResult_f103_m1
.
fxs:EMA_34_1 pip:hasIndicatorResult pip:EMAResult_s103_m1
.
fxs:Ask_1
  rdf:type pip:Ask ;
  pip:Value '89.3'^^xsd:float ;
  pip:TimeStamp '2015-10-19T14:08:17.5Z' ;
.
fxs:Bid_1
  rdf:type pip:Bid ;
  pip:Value '89.1'^^xsd:float ;
  pip:TimeStamp '2015-10-19T14:08:17.5Z' ;
.
fxs:PriceFastSlowPattern_1
		pip:hasAsk	fxs:Ask_1 ;
		pip:hasBid	fxs:Bid_1 ;
		.
fxs:Ask_2
  rdf:type pip:Ask ;
  pip:Value '89.4'^^xsd:float ;
  pip:TimeStamp '2015-10-19T14:09:17.5Z' ;
.
fxs:Bid_2
  rdf:type pip:Bid ;
  pip:Value '89.2'^^xsd:float ;
  pip:TimeStamp '2015-10-19T14:09:17.5Z' ;
.
fxs:PriceFastSlowPattern_1
		pip:hasAsk	fxs:Ask_2 ;
		pip:hasBid	fxs:Bid_2 ;
		.


";


            ruleEngine.insertModel(prolog + string.Format(s1, Pair));
            ModelWrapper mw = ruleEngine.run();
            Assert.AreEqual(result, ruleEngine.hasOutcomeResults());


        }

        void emam5(bool result)
        {

            Triple.remove(ruleEngine.getModel(), "pip:RuleContext"); // Change the context!!

            String s2 = @"
		pip:RuleContext
		rdf:type pip:RuleContextSingleton ;
		pip:hasInstrument pip:{0} ;
		pip:hasTimeFrame pip:m5 ;
.

pip:EMAResult_f102
  rdf:type pip:EMAResult ;
  pip:Value '89.2'^^xsd:float ;
  pip:TimeStamp '2015-10-19T14:08:17.5Z';
.
pip:EMAResult_s102
          rdf:type pip:EMAResult;
            pip:Value '89.3' ^^ xsd:float ;
            pip:TimeStamp '2015-10-19T14:08:17.639Z';
.
fxs:EMA_8 pip:hasIndicatorResult pip:EMAResult_f102
 .
 fxs:EMA_34 pip:hasIndicatorResult pip:EMAResult_s102
 .
 pip:EMAResult_f103
   rdf:type pip:EMAResult;
            pip:Value '89.4' ^^ xsd:float ;
            pip:TimeStamp '2015-10-19T14:09:17.5Z';
.
pip:EMAResult_s103
          rdf:type pip:EMAResult;
            pip:Value '89.2' ^^ xsd:float ;
            pip:TimeStamp '2015-10-19T14:09:17.639Z';
.
fxs:EMA_8 pip:hasIndicatorResult pip:EMAResult_f103
 .
 fxs:EMA_34 pip:hasIndicatorResult pip:EMAResult_s103
 .


";
            ruleEngine.insertModel(prolog + string.Format(s2, Pair));


            ModelWrapper mw = ruleEngine.run();
            Assert.AreEqual(result, ruleEngine.hasOutcomeResults());

        }

        void rsim5(bool result)
        {


            Triple.remove(ruleEngine.getModel(), "pip:RuleContext"); // Change the context!!

            String s3 = @"

		pip:RuleContext
		rdf:type pip:RuleContextSingleton ;
		pip:hasInstrument pip:{0} ;
		pip:hasTimeFrame pip:m5 ;
.



pip:RSIResult_100b
  rdf:type pip:RSIResult ;
  pip:Value '69.2'^^xsd:float ;
  pip:TimeStamp '2015-10-19T15:09:17.639Z';
.
fxs:RSI_9 pip:hasIndicatorResult pip:RSIResult_100b;
.

pip:RSIResult_101b
          rdf:type pip:RSIResult;
            pip:Value '79.2' ^^ xsd:float ;
            pip:TimeStamp '2015-10-19T15:09:18.639Z';
.
fxs:RSI_9 pip:hasIndicatorResult pip:RSIResult_101b;
.

";
            ruleEngine.insertModel(prolog + string.Format(s3, Pair));
            ModelWrapper mw = ruleEngine.run();
            Assert.AreEqual(result, ruleEngine.hasOutcomeResults());

        }
        public void scope(Model model)
        {
            sparqlScope.SparqlScope scope = new sparqlScope.SparqlScope(model);
            scope.ShowDialog(this);

        }

    }
}

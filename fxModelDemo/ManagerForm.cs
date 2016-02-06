using System;
using System.Collections.Generic;
using System.Windows.Forms;
using fxCoreLink;
using Common;
using Common.fx;
using csExperts;
using System.Reflection;
using System.Configuration;
using simulatedTrading;

namespace fxModel
{
    public partial class ManagerForm : Form, Display, Common.Control, Output
    {
        #region lifecycle
        private Logger log = Logger.LogManager("ManagerForm");
        private PriceProcessor priceProcessor;
        private IFXManager fxManager;
        private IFxUpdates fxUpdates;
        private AccumulatorMgr accumulatorMgr;
        private bool ErrorState = false; // needs implementation
        private MailSender mailSender = new MailSender();
        private string pyfile = "macdSignal.py";
        private ExpertFactory expertFactory;
        private static Dictionary<string, List<string>> curMap = new Dictionary<string, List<string>>();
        private Dictionary<string, RadioButton> mapTimeFrameRadio = new Dictionary<string, RadioButton>();
        private string projectFolder = Environment.CurrentDirectory;
        private bool simulated = false;
        private string user;
        private string pw;
        private string url;
        private string conn;

        static ManagerForm()
        {
            foreach (string k in new string[] { "AUD", "CAD", "EUR", "GBP", "JPY", "NZD", "USD" })
            {
                curMap.Add(k, new List<string>());
                List<string> l = curMap[k];
                if (k == "AUD") l.AddRange(new string[] { "AUD/CAD", "AUD/JPY", "AUD/NZD", "AUD/USD", "EUR/AUD", "GBP/AUD" });
                if (k == "CAD") l.AddRange(new string[] { "AUD/CAD", "CAD/JPY", "EUR/CAD", "GBP/CAD", "USD/CAD" });
                if (k == "EUR") l.AddRange(new string[] { "EUR/AUD", "EUR/CAD", "EUR/GBP", "EUR/JPY", "EUR/USD", "EUR/CHF" });
                if (k == "GBP") l.AddRange(new string[] { "EUR/GBP", "GBP/AUD", "GBP/CAD", "GBP/JPY", "GBP/NZD", "GBP/USD" });
                if (k == "JPY") l.AddRange(new string[] { "AUD/JPY", "EUR/JPY", "GBP/JPY", "NZD/JPY", "USD/JPY" });
                if (k == "NZD") l.AddRange(new string[] { "AUD/NZD", "GBP/NZD", "NZD/JPY", "NZD/USD" });
                if (k == "USD") l.AddRange(new string[] { "AUD/USD", "EUR/USD", "GBP/USD", "NZD/USD", "USD/CAD", "USD/JPY", "USD/CHF", "USD/CNH" });
            }
        }

        public ManagerForm()
        {
            projectFolder = ConfigurationManager.AppSettings["projectFolder"];

            InitializeComponent();

            mapTimeFrameRadio.Add("m1", this.radioButtonM1);
            mapTimeFrameRadio.Add("m5", this.radioButtonM5);
            mapTimeFrameRadio.Add("m15", this.radioButtonM15);
            mapTimeFrameRadio.Add("m30", this.radioButtonM30);
            mapTimeFrameRadio.Add("H1", this.radioButtonH1);
            mapTimeFrameRadio.Add("H4", this.radioButtonH4);
            mapTimeFrameRadio.Add("D1", this.radioButtonD1);

            new ToolTip().SetToolTip(this.groupBoxPairs, "Select pairs to run strategies with experts");
            new ToolTip().SetToolTip(this.groupBoxJournal, "Read or write a journal of price and \r\ntime period data when running experts");
            new ToolTip().SetToolTip(this.groupBoxCs, "Select and run a C# expert");
            new ToolTip().SetToolTip(this.groupBoxPython, "Select and run a Python expert");
            new ToolTip().SetToolTip(this.groupBoxTimeframe, "Choose the maximum timeframe the expert is called for");

        }

        private void ManagerForm_Load(object sender, EventArgs e)
        {
            // initialize UI from properties

            List<string> selectedPairs = new List<string>();
            selectedPairs.AddRange(Property.getInstance().getDelimitedListProperty("selectedPairs"));
            if (selectedPairs.Count > 0)
            {
                this.checkedListBoxPairs.Items.Clear();
                foreach (string s in selectedPairs)
                {
                    int i = this.checkedListBoxPairs.Items.Add(s);
                    this.checkedListBoxPairs.SetItemChecked(i, true);
                }
            }

            List<string> selectedTimeFrames = new List<string>();
            selectedTimeFrames.AddRange(Property.getInstance().getDelimitedListProperty("selectedTimeFrames"));
            string defaultTimeFrame = Property.getInstance().getProperty("defaultTimeFrame","m1");
            mapTimeFrameRadio[defaultTimeFrame].Checked = true;


            setupCsExperts();

            string user = Property.getInstance().getProperty("user");
            string pw = Property.getInstance().getProperty("pw");
            string url = Property.getInstance().getProperty("url");
            string conn = Property.getInstance().getProperty("connection");
            this.toolStripStatusLabelAccount.Text = user;
            this.toolStripStatusLabelConnection.Text = conn;

            accumulatorMgr = new AccumulatorMgr();

            try
            {
                if ("simulated" == conn.ToLower())
                {
                    fxManager = new FXManagerSimulated();
                    simulated = true;
                    fxUpdates = new FxUpdatesSimulated(this, this, fxManager, accumulatorMgr);
                }
                else {
                    fxManager = new FXManager(this, user, pw, url, conn, mailSender);
                    fxUpdates = new FxUpdates(this, this, fxManager, accumulatorMgr);
                }
            }
            catch (System.ComponentModel.WarningException ex)
            {
                MessageBox.Show("No connection: " + ex.Message + "; simulation mode only");
                fxManager = new FXManagerSimulated();
                simulated = true;
                fxUpdates = new FxUpdates(this, this, fxManager, accumulatorMgr);
                this.toolStripStatusLabelConnection.Text = "simulation";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal error: " + ex.Message + "; cannot continue");
                return;
            }

            pyfile = Property.getInstance().getProperty("pyfile",pyfile);
            this.textBoxPyFile.Text = pyfile;
            this.buttonPython.Enabled = true;
        }

        private void ManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (fxManager != null)
            {
                fxManager.closeSession();
            }
            if (priceProcessor != null)
            {
                priceProcessor.stop();
            }
            if (fxUpdates != null)
            {
                fxUpdates.setCanProcess(false);
            }

        }
        #endregion
        #region interface implementation
        public void listUpdate(List<string> list)
        {
            throw new NotImplementedException();
        }

        public void display(string s)
        {
            this.textBoxDisplay.Text = s;
        }

        // THIS is a great UI cross-thread delegate approach
        public delegate void serviceGUIDelegate(string msg);
        public void appendDelegate(string s)
        {
            this.textBoxDisplay.AppendText(s);
        }

        public void append(string s)
        {
            try
            {
                this.Invoke(new serviceGUIDelegate(appendDelegate), s);
                //log.info(s.Replace("\r\n", ";"));
            }
            catch (InvalidOperationException e)
            {
            }
        }

        public void appendLine(string s)
        {
            this.append(s + "\r\n");
        }
        public void appendLine(string s, params object[] values)
        {
            this.appendLine(string.Format(s, values));
        }

        public void setStatus(string s)
        {
            //this.toolStripStatusLabel1.Text = s;
        }

        public void logger(string s)
        {
            log.info(s);
        }

        public bool ordersArmed()
        {
            return Property.getInstance().getBooleanProperty("ordersArmed", false); ;
        }

        public bool isExcludedPair(string pair)
        {
            return false;
        }

        public bool isProcessingAllowed()
        {
            return true;
        }

        public bool isErrorState()
        {
            return ErrorState;
        }

        public bool isValidPair(string pair)
        {
            return this.checkedListBoxPairs.CheckedItems.Contains(pair);
        }

        public bool isJournalRead()
        {
            return this.radioButtonJournalRead.Checked;
        }
        public bool isJournalWrite()
        {
            return this.radioButtonJournalWrite.Checked;
        }

        // THIS is a great UI cross-thread delegate approach
        public delegate void serviceGUIDelegate2(bool value);
        public void runStatusDelegate(bool value)
        {
            this.runStatus(value);
        }

        public void journalReadDone()
        {
            expertFactory.shutdown();
            this.Invoke(new serviceGUIDelegate2(runStatusDelegate), false);

#if false
            // TODO-review for release
            try
            {
                List<string> selectedPairs = new List<string>();
                selectedPairs.AddRange(Property.getInstance().getDelimitedListProperty("selectedPairs"));
                double totalPips = 0.0;
                double totalWin = 0.0;
                double totalLoss = 0.0;
                int count = 0;
                int countWin = 0;
                int countLoss = 0;
                foreach (string pair in selectedPairs) {
                    Dictionary < string, object> mapResult = TransactionManager.getInstance().getClosedTrade().getClosedPositionGross(pair);
                    double pips = (double)mapResult["pips"];
                    totalPips += pips;
                    totalWin += (double)mapResult["pipsWin"];
                    totalLoss += (double)mapResult["pipsLoss"];
                    count += (int)mapResult["count"];
                    countWin += (int)mapResult["wins"];
                    countLoss += (int)mapResult["losses"];
                    string result = string.Format("{0,-15}{1}\t{2}\t{3}\t{4}\t{5}\t{6}", 
                        pair, 
                        pips.ToString("F1"),
                        ((double)mapResult["pipsWin"]).ToString("F1"),
                        ((double)mapResult["pipsLoss"]).ToString("F1"),
                        ((double)mapResult["count"]).ToString("F1"),
                        mapResult["wins"],
                        mapResult["losses"]
                        );
                    Console.WriteLine(result);
                    appendLine(result);
                    log.debug(result);
                }
                string result2 = string.Format("{0,-15}{1}\t{2}\t{3}\t{4}\t{5}\t{6}", 
                    "Total", 
                    totalPips.ToString("F1"),
                    totalWin.ToString("F1"),
                    totalLoss.ToString("F1"), 
                    count.ToString(), 
                    countWin.ToString(), 
                    countLoss.ToString()
                    );
                Console.WriteLine(result2);
                appendLine(result2);
                log.debug(result2);

                TransactionManager.snapshot();
            }
            catch (InvalidOperationException e)
            {
            }
#endif
        }

        public string getProperty(string name, string dflt)
        {
            return Property.getInstance().getProperty(name);
        }

        public bool getBoolProperty(string name, bool dflt)
        {
            return Property.getInstance().getBooleanProperty(name, dflt);
        }

        public int getIntProperty(string name, int dflt)
        {
            return Property.getInstance().getIntProperty(name, dflt);
        }

        public double getDoubleProperty(string name, double dflt)
        {
            return Property.getInstance().getDoubleProperty(name, dflt);
        }

        public Dictionary<string, object> getPairParams(string pair)
        {
            throw new NotImplementedException();
        }
#endregion

#region behaviors


        private void buttonPickPairs_Click(object sender, EventArgs e)
        {
            string cur = this.comboBoxCurrencies.Text;
            if (cur == null || cur == "")
                return;
            for (int i = 0; i < this.checkedListBoxPairs.Items.Count; i++)
                this.checkedListBoxPairs.SetItemChecked(i, false);
            List<string> lp = curMap[cur];
            for (int i = 0; i < this.checkedListBoxPairs.Items.Count; i++)
            {
                string p = (string)this.checkedListBoxPairs.Items[i];
                if (lp.Contains(p))
                    this.checkedListBoxPairs.SetItemChecked(i, true);
            }
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBoxPairs.Items.Count; i++)
                this.checkedListBoxPairs.SetItemChecked(i, true);
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBoxPairs.Items.Count; i++)
                this.checkedListBoxPairs.SetItemChecked(i, false);
        }

        private void buttonPyFileDialog_Click(object sender, EventArgs e)
        {
            if (this.textBoxPyFile.Text.Trim() == "")
            {
                this.openFileDialogPy.FileName = "*.py";
            }
            else
            {
                this.openFileDialogPy.FileName = textBoxPyFile.Text;
            }
            this.openFileDialogPy.InitialDirectory = projectFolder;
            DialogResult dr = this.openFileDialogPy.ShowDialog();
            if (DialogResult.OK == dr)
            {
                pyfile = this.openFileDialogPy.FileName;
                this.textBoxPyFile.Text = pyfile;
            }
        }

        private void buttonPython_Click(object sender, EventArgs e)
        {
            List<string> pairs = new List<string>();
            foreach (string p in this.checkedListBoxPairs.CheckedItems)
            {
                if (!isValidPair(p))
                {
                    log.error("timedOCOExpertEvent(), invalid pair {0}", p);
                    continue;
                }
                pairs.Add(p);
            }
            if (pairs.Count == 0)
            {
                MessageBox.Show("No pairs selected");
                return;
            }

            string timeFrame = "m1";
            foreach (string t in mapTimeFrameRadio.Keys)
            {
                if (mapTimeFrameRadio[t].Checked)
                {
                    timeFrame = t;
                }
            }

            runExpert(pairs,timeFrame);
        }

        private void runExpert(List<string> pairs,string timeFrame)
        {
            try
            {
                priceProcessor = new PriceProcessor(this, this, fxManager, fxUpdates);
                expertFactory = new ExpertFactory(priceProcessor);
                Clock.Source = expertFactory;
                if (isJournalRead() || isJournalWrite())
                {
                    accumulatorMgr.Snapshot = getProperty("journalFile", "testdata");
                    expertFactory.setJournal(
                        pairs, 
                        getProperty("journalFile", "testdata"));
                }
                if (isJournalRead())
                {
                    accumulatorMgr.read();  // load history
                    log.debug("PriceHistory done");
                }

                foreach (string p in pairs)
                {
                    TimeFrame tf = ExpertFactory.TimeFrameInverseMap[timeFrame];
                    ExpertScript es = new ExpertScript(pyfile, p, tf, true);
                    expertFactory.subscribe(es);
                    expertFactory.subscribePrice(es);
                }

                expertFactory.startExperts();
                runStatus(true);

                if (isJournalRead())
                    expertFactory.readJournal();
                else
                {
                    priceProcessor.start(pairs); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error running expert: " + ex.Message);
                return;
            }
        }

        private void buttonStopExperts_Click(object sender, EventArgs e)
        {
            expertFactory.shutdown();
            runStatus(false);
        }

        public void runStatus(bool status)
        {
            this.buttonPython.Enabled = !status;
            this.buttonCsRun.Enabled = !status;
            this.buttonStopExperts.Enabled = status;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Version version0 = typeof(ManagerForm).Assembly.GetName().Version;
            Version version1 = typeof(AccumulatorMgr).Assembly.GetName().Version;
            
            MessageBox.Show(string.Format(
                "{0}, v.{1}\r\n{2} v.{3}\r\n{4}",
                "PipSeq Demo",
                version0,
                "fxCoreLink",
                version1,
                "www.pipseq.org"));
        }
#endregion

        private void buttonCsRun_Click(object sender, EventArgs e)
        {

            List<string> pairs = new List<string>();
            foreach (string p in this.checkedListBoxPairs.CheckedItems)
            {
                if (!isValidPair(p))
                {
                    log.error("timedOCOExpertEvent(), invalid pair {0}", p);
                    continue;
                }
                pairs.Add(p);
            }
            if (pairs.Count == 0)
            {
                MessageBox.Show("No pairs selected");
                return;
            }

            string timeFrame = "m1";
            foreach (string t in mapTimeFrameRadio.Keys)
            {
                if (mapTimeFrameRadio[t].Checked)
                {
                    timeFrame = t;
                }
            }


            runCsExpert(pairs, timeFrame);

        }

        private void runCsExpert(List<string> pairs, string timeFrame)
        {
            try
            {
                priceProcessor = new PriceProcessor(this, this, fxManager, fxUpdates);
                expertFactory = new ExpertFactory(priceProcessor);
                Clock.Source = expertFactory;
                if (isJournalRead() || isJournalWrite())
                {
                    accumulatorMgr.Snapshot = getProperty("journalFile", "testdata");
                    expertFactory.setJournal(
                        pairs,
                        getProperty("journalFile", "testdata"));
                }
                if (isJournalRead())
                {
                    accumulatorMgr.read();  // load history
                    log.debug("PriceHistory done");
                }

                ConstructorInfo ci = null;
                string exp = this.comboBoxExpertAttribs.Text;
                string[] ea = exp.Split(';');
                if (ea.Length > 0)
                {
                    string expert = ea[0];
                    Type type = Type.GetType(expert);
                    ci = type.GetConstructor(new Type[] { typeof(string), typeof(TimeFrame) });
                    if (ci == null)
                    {
                        log.error("Invalid or no constructor for selected csExpert " + expert);
                        return;
                    }
                }
                if (simulated)
                foreach (string p in pairs)
                {
                    //TimeFrame tf = ExpertFactory.TimeFrameInverseMap[timeFrame];
                    ExpertBase es = new PriceSimulator(p, TimeFrame.m1);
                    //expertFactory.subscribe(es);
                    expertFactory.subscribePrice(es);
                }
                foreach (string p in pairs)
                {
                    TimeFrame tf = ExpertFactory.TimeFrameInverseMap[timeFrame];
                    ExpertBase es = (ExpertBase)ci.Invoke(new object[] { p, tf });
                    expertFactory.subscribe(es);
                    expertFactory.subscribePrice(es);
                }

                expertFactory.startExperts();
                runStatus(true);

                if (isJournalRead())
                    expertFactory.readJournal();
                else
                {
                    priceProcessor.start(pairs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error running expert: " + ex.Message);
                return;
            }

        }

        private void setupCsExperts()
        {
            Dictionary<string,string> map = ExpertAttribute.getClasses();
            int i = 0;
            foreach (string exp in map.Keys)
            {
                this.comboBoxExpertAttribs.Items.Add(
                    string.Format("{0}; {1}", exp, map[exp]));
                if (i++ == 0)
                    this.comboBoxExpertAttribs.Text =
                        this.comboBoxExpertAttribs.Items[0].ToString();
            }
        }

        private void comboBoxExpertAttribs_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text = comboBoxExpertAttribs.Text;
            string[] ea = text.Split(';');
            if (ea.Length > 0)
            {
                this.labelCsExpertDescription.Text = ea[1];
            }

        }

        public void progress(long current, long max)
        {
            long x = max / 100;
            long v = current / x;
            progress((int)v);
        }
        public delegate void progressGUIDelegate(int value);
        public void progressDelegate(int value)
        {
            this.toolStripProgressBar1.Value = value;
        }

        public void progress(int value)
        {
            try
            {
                this.Invoke(new progressGUIDelegate(progressDelegate), value);
            }
            catch (InvalidOperationException e)
            {
            }
        }

        public void scopeDelegate(object model)
        {
            sparqlScope.SparqlScope scope = new sparqlScope.SparqlScope(model);
            scope.ShowDialog();

            //org.pipseq.rdf.jena.util.SparqlScope.scope((com.hp.hpl.jena.rdf.model.Model)model);
        }
        public delegate void scopeGUIDelegate(object model);

        public void scope(object model)
        {
            try
            {
                this.Invoke(new scopeGUIDelegate(scopeDelegate), model);
            }
            catch (InvalidOperationException e)
            {
            }
        }

        private void radioButtonJournalRead_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonJournalRead.Checked)
            {
                fxManager = new FXManagerSimulated();
                fxUpdates = new FxUpdatesSimulated(this, this, fxManager, accumulatorMgr);
            }
        }

        private void radioButtonJournalWrite_CheckedChanged(object sender, EventArgs e)
        {
            if (!simulated 
                && user != null
                && this.radioButtonJournalWrite.Checked)
            {
                fxManager = new FXManager(this, user, pw, url, conn, mailSender);
                fxUpdates = new FxUpdates(this, this, fxManager, accumulatorMgr);
            }
        }

        private void radioButtonNoJournal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonNoJournal.Checked)
            {
                if (simulated)
                {
                    fxManager = new FXManagerSimulated();
                    fxUpdates = new FxUpdatesSimulated(this, this, fxManager, accumulatorMgr);
                }
                else if (user != null)
                {
                    fxManager = new FXManager(this, user, pw, url, conn, mailSender);
                    fxUpdates = new FxUpdates(this, this, fxManager, accumulatorMgr);
                }
            }
        }
    }
}

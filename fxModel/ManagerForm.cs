using System;
using System.Collections.Generic;
using System.Windows.Forms;
using fxCoreLink;
using Common;

namespace fxModel
{
    public partial class ManagerForm : Form, Display, fxCoreLink.Control, Output
    {
        #region lifecycle
        private Logger log = Logger.LogManager("ManagerForm");
        private PriceProcessor priceProcessor;
        private IFXManager fxManager;
        private FxUpdates fxUpdates;
        private AccumulatorMgr accumulatorMgr;
        private bool ErrorState = false; // needs implementation
        private MailSender mailSender = new MailSender();
        private string pyfile = "macdSignal.py";
        private ExpertFactory expertFactory;
        private static Dictionary<string, List<string>> curMap = new Dictionary<string, List<string>>();
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
            Property.getInstance().loadProperties(@"..\..\..\var\config\properties.xml");
            InitializeComponent();
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

            string user = Property.getInstance().getProperty("user");
            string pw = Property.getInstance().getProperty("pw");
            string url = Property.getInstance().getProperty("url");
            string conn = Property.getInstance().getProperty("connection");

            accumulatorMgr = new AccumulatorMgr();

            try
            {
                if ("simulated" == conn.ToLower())
                {
                    fxManager = new SimFXManager();
                } else {
                    fxManager = new FXManager(this, user, pw, url, conn, mailSender);
                }
                fxUpdates = new FxUpdates(this, this, fxManager, accumulatorMgr);
            }
            catch (System.ComponentModel.WarningException ex)
            {
                MessageBox.Show("No connection: " + ex.Message + "; simulation mode only");
                fxManager = new SimFXManager();
                fxUpdates = new FxUpdates(this, this, fxManager, accumulatorMgr);
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
                fxUpdates.canProcess = false;
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
            try
            {
                this.Invoke(new serviceGUIDelegate2(runStatusDelegate), false);
            }
            catch (InvalidOperationException e)
            {
            }
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
            if (this.textBoxPyFile.Text.Trim() != "")
            {
                this.openFileDialogPy.FileName = "*.py";// pyfile;
            }
            else
            {
                pyfile = textBoxPyFile.Text;
            }
            DialogResult dr = this.openFileDialogPy.ShowDialog();
            if (DialogResult.OK == dr)
            {
                pyfile = this.openFileDialogPy.SafeFileName;
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

            runExpert(pairs);

        }

        private void runExpert(List<string> pairs)
        {
            try
            {
                priceProcessor = new PriceProcessor(this, this, fxManager, fxUpdates);
                expertFactory = new ExpertFactory(priceProcessor);
                if (isJournalRead() || isJournalWrite())
                {
                    accumulatorMgr.Snapshot = getProperty("journalFile", "testdata");
                    expertFactory.setJournal(
                        pairs, 
                        getProperty("journalFile", "testdata"),
                        this.checkBoxAppend.Checked);
                }
                if (isJournalRead())
                {
                    accumulatorMgr.read();  // load history
                    log.debug("PriceHistory done");
                }

                foreach (string p in pairs)
                {
                    ExpertScript es = new ExpertScript(pyfile, p, TimeFrame.m5, true);
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

        private void buttonStopPython_Click(object sender, EventArgs e)
        {
            expertFactory.shutdown();
            runStatus(false);
        }

        private void runStatus(bool status)
        {
            this.buttonPython.Enabled = !status;
            this.buttonStopPython.Enabled = status;
        }

        private void radioButtonJournalWrite_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBoxAppend.Enabled = this.radioButtonJournalWrite.Checked;
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
    }
}

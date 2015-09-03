namespace fxModel
{
    partial class ManagerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonPickPairs = new System.Windows.Forms.Button();
            this.comboBoxCurrencies = new System.Windows.Forms.ComboBox();
            this.buttonClearAll = new System.Windows.Forms.Button();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.checkedListBoxPairs = new System.Windows.Forms.CheckedListBox();
            this.label16 = new System.Windows.Forms.Label();
            this.buttonPyFileDialog = new System.Windows.Forms.Button();
            this.textBoxPyFile = new System.Windows.Forms.TextBox();
            this.buttonPython = new System.Windows.Forms.Button();
            this.openFileDialogPy = new System.Windows.Forms.OpenFileDialog();
            this.textBoxDisplay = new System.Windows.Forms.TextBox();
            this.groupBoxPython = new System.Windows.Forms.GroupBox();
            this.buttonStopExperts = new System.Windows.Forms.Button();
            this.groupBoxJournal = new System.Windows.Forms.GroupBox();
            this.radioButtonJournalRead = new System.Windows.Forms.RadioButton();
            this.radioButtonJournalWrite = new System.Windows.Forms.RadioButton();
            this.radioButtonNoJournal = new System.Windows.Forms.RadioButton();
            this.groupBoxPairs = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonCsRun = new System.Windows.Forms.Button();
            this.comboBoxExpertAttribs = new System.Windows.Forms.ComboBox();
            this.groupBoxCs = new System.Windows.Forms.GroupBox();
            this.labelCsExpertDescription = new System.Windows.Forms.Label();
            this.groupBoxTimeframe = new System.Windows.Forms.GroupBox();
            this.radioButtonD1 = new System.Windows.Forms.RadioButton();
            this.radioButtonH4 = new System.Windows.Forms.RadioButton();
            this.radioButtonH1 = new System.Windows.Forms.RadioButton();
            this.radioButtonM30 = new System.Windows.Forms.RadioButton();
            this.radioButtonM15 = new System.Windows.Forms.RadioButton();
            this.radioButtonM5 = new System.Windows.Forms.RadioButton();
            this.radioButtonM1 = new System.Windows.Forms.RadioButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelAccount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelConnection = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBoxPython.SuspendLayout();
            this.groupBoxJournal.SuspendLayout();
            this.groupBoxPairs.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBoxCs.SuspendLayout();
            this.groupBoxTimeframe.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPickPairs
            // 
            this.buttonPickPairs.Location = new System.Drawing.Point(10, 80);
            this.buttonPickPairs.Margin = new System.Windows.Forms.Padding(4);
            this.buttonPickPairs.Name = "buttonPickPairs";
            this.buttonPickPairs.Size = new System.Drawing.Size(63, 28);
            this.buttonPickPairs.TabIndex = 59;
            this.buttonPickPairs.Text = "Pairs";
            this.buttonPickPairs.UseVisualStyleBackColor = true;
            this.buttonPickPairs.Click += new System.EventHandler(this.buttonPickPairs_Click);
            // 
            // comboBoxCurrencies
            // 
            this.comboBoxCurrencies.FormattingEnabled = true;
            this.comboBoxCurrencies.Items.AddRange(new object[] {
            "AUD",
            "CAD",
            "EUR",
            "GBP",
            "JPY",
            "NZD",
            "USD"});
            this.comboBoxCurrencies.Location = new System.Drawing.Point(10, 47);
            this.comboBoxCurrencies.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxCurrencies.Name = "comboBoxCurrencies";
            this.comboBoxCurrencies.Size = new System.Drawing.Size(61, 24);
            this.comboBoxCurrencies.TabIndex = 58;
            // 
            // buttonClearAll
            // 
            this.buttonClearAll.Location = new System.Drawing.Point(11, 152);
            this.buttonClearAll.Margin = new System.Windows.Forms.Padding(4);
            this.buttonClearAll.Name = "buttonClearAll";
            this.buttonClearAll.Size = new System.Drawing.Size(73, 28);
            this.buttonClearAll.TabIndex = 57;
            this.buttonClearAll.Text = "Clear all";
            this.buttonClearAll.UseVisualStyleBackColor = true;
            this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
            // 
            // buttonSelectAll
            // 
            this.buttonSelectAll.Location = new System.Drawing.Point(11, 116);
            this.buttonSelectAll.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Size = new System.Drawing.Size(83, 28);
            this.buttonSelectAll.TabIndex = 56;
            this.buttonSelectAll.Text = "Select all";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // checkedListBoxPairs
            // 
            this.checkedListBoxPairs.CheckOnClick = true;
            this.checkedListBoxPairs.FormattingEnabled = true;
            this.checkedListBoxPairs.Items.AddRange(new object[] {
            "AUD/CAD",
            "AUD/JPY",
            "AUD/NZD",
            "AUD/USD",
            "CAD/JPY",
            "EUR/AUD",
            "EUR/CAD",
            "EUR/GBP",
            "EUR/JPY",
            "EUR/USD",
            "GBP/AUD",
            "GBP/CAD",
            "GBP/JPY",
            "GBP/NZD",
            "GBP/USD",
            "NZD/JPY",
            "NZD/USD",
            "USD/CAD",
            "USD/JPY"});
            this.checkedListBoxPairs.Location = new System.Drawing.Point(102, 22);
            this.checkedListBoxPairs.Margin = new System.Windows.Forms.Padding(4);
            this.checkedListBoxPairs.Name = "checkedListBoxPairs";
            this.checkedListBoxPairs.Size = new System.Drawing.Size(128, 208);
            this.checkedListBoxPairs.TabIndex = 55;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 37);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(26, 17);
            this.label16.TabIndex = 63;
            this.label16.Text = "file";
            // 
            // buttonPyFileDialog
            // 
            this.buttonPyFileDialog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPyFileDialog.Location = new System.Drawing.Point(172, 32);
            this.buttonPyFileDialog.Margin = new System.Windows.Forms.Padding(4);
            this.buttonPyFileDialog.Name = "buttonPyFileDialog";
            this.buttonPyFileDialog.Size = new System.Drawing.Size(33, 25);
            this.buttonPyFileDialog.TabIndex = 62;
            this.buttonPyFileDialog.Text = "...";
            this.buttonPyFileDialog.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonPyFileDialog.UseVisualStyleBackColor = true;
            this.buttonPyFileDialog.Click += new System.EventHandler(this.buttonPyFileDialog_Click);
            // 
            // textBoxPyFile
            // 
            this.textBoxPyFile.Location = new System.Drawing.Point(46, 34);
            this.textBoxPyFile.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPyFile.Name = "textBoxPyFile";
            this.textBoxPyFile.Size = new System.Drawing.Size(124, 22);
            this.textBoxPyFile.TabIndex = 61;
            // 
            // buttonPython
            // 
            this.buttonPython.BackColor = System.Drawing.SystemColors.Control;
            this.buttonPython.Enabled = false;
            this.buttonPython.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPython.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.buttonPython.Location = new System.Drawing.Point(6, 68);
            this.buttonPython.Margin = new System.Windows.Forms.Padding(4);
            this.buttonPython.Name = "buttonPython";
            this.buttonPython.Size = new System.Drawing.Size(66, 28);
            this.buttonPython.TabIndex = 60;
            this.buttonPython.Text = "Run";
            this.buttonPython.UseVisualStyleBackColor = false;
            this.buttonPython.Click += new System.EventHandler(this.buttonPython_Click);
            // 
            // openFileDialogPy
            // 
            this.openFileDialogPy.FileName = "openFileDialog1";
            // 
            // textBoxDisplay
            // 
            this.textBoxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDisplay.Location = new System.Drawing.Point(12, 285);
            this.textBoxDisplay.Multiline = true;
            this.textBoxDisplay.Name = "textBoxDisplay";
            this.textBoxDisplay.ReadOnly = true;
            this.textBoxDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDisplay.Size = new System.Drawing.Size(710, 173);
            this.textBoxDisplay.TabIndex = 64;
            this.textBoxDisplay.WordWrap = false;
            // 
            // groupBoxPython
            // 
            this.groupBoxPython.Controls.Add(this.textBoxPyFile);
            this.groupBoxPython.Controls.Add(this.buttonPython);
            this.groupBoxPython.Controls.Add(this.label16);
            this.groupBoxPython.Controls.Add(this.buttonPyFileDialog);
            this.groupBoxPython.Location = new System.Drawing.Point(281, 31);
            this.groupBoxPython.Name = "groupBoxPython";
            this.groupBoxPython.Size = new System.Drawing.Size(212, 108);
            this.groupBoxPython.TabIndex = 65;
            this.groupBoxPython.TabStop = false;
            this.groupBoxPython.Text = "Python expert";
            // 
            // buttonStopExperts
            // 
            this.buttonStopExperts.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStopExperts.Enabled = false;
            this.buttonStopExperts.ForeColor = System.Drawing.Color.DarkRed;
            this.buttonStopExperts.Location = new System.Drawing.Point(499, 171);
            this.buttonStopExperts.Name = "buttonStopExperts";
            this.buttonStopExperts.Size = new System.Drawing.Size(117, 30);
            this.buttonStopExperts.TabIndex = 64;
            this.buttonStopExperts.Text = "Stop Experts";
            this.buttonStopExperts.UseVisualStyleBackColor = false;
            this.buttonStopExperts.Click += new System.EventHandler(this.buttonStopExperts_Click);
            // 
            // groupBoxJournal
            // 
            this.groupBoxJournal.Controls.Add(this.radioButtonJournalRead);
            this.groupBoxJournal.Controls.Add(this.radioButtonJournalWrite);
            this.groupBoxJournal.Controls.Add(this.radioButtonNoJournal);
            this.groupBoxJournal.Location = new System.Drawing.Point(499, 31);
            this.groupBoxJournal.Name = "groupBoxJournal";
            this.groupBoxJournal.Size = new System.Drawing.Size(94, 113);
            this.groupBoxJournal.TabIndex = 72;
            this.groupBoxJournal.TabStop = false;
            this.groupBoxJournal.Text = "Journal";
            // 
            // radioButtonJournalRead
            // 
            this.radioButtonJournalRead.AutoSize = true;
            this.radioButtonJournalRead.Location = new System.Drawing.Point(6, 59);
            this.radioButtonJournalRead.Name = "radioButtonJournalRead";
            this.radioButtonJournalRead.Size = new System.Drawing.Size(63, 21);
            this.radioButtonJournalRead.TabIndex = 69;
            this.radioButtonJournalRead.Text = "Read";
            this.radioButtonJournalRead.UseVisualStyleBackColor = true;
            // 
            // radioButtonJournalWrite
            // 
            this.radioButtonJournalWrite.AutoSize = true;
            this.radioButtonJournalWrite.Location = new System.Drawing.Point(6, 86);
            this.radioButtonJournalWrite.Name = "radioButtonJournalWrite";
            this.radioButtonJournalWrite.Size = new System.Drawing.Size(62, 21);
            this.radioButtonJournalWrite.TabIndex = 70;
            this.radioButtonJournalWrite.Text = "Write";
            this.radioButtonJournalWrite.UseVisualStyleBackColor = true;
            this.radioButtonJournalWrite.CheckedChanged += new System.EventHandler(this.radioButtonJournalWrite_CheckedChanged);
            // 
            // radioButtonNoJournal
            // 
            this.radioButtonNoJournal.AutoSize = true;
            this.radioButtonNoJournal.Checked = true;
            this.radioButtonNoJournal.Location = new System.Drawing.Point(6, 32);
            this.radioButtonNoJournal.Name = "radioButtonNoJournal";
            this.radioButtonNoJournal.Size = new System.Drawing.Size(63, 21);
            this.radioButtonNoJournal.TabIndex = 68;
            this.radioButtonNoJournal.TabStop = true;
            this.radioButtonNoJournal.Text = "None";
            this.radioButtonNoJournal.UseVisualStyleBackColor = true;
            // 
            // groupBoxPairs
            // 
            this.groupBoxPairs.Controls.Add(this.checkedListBoxPairs);
            this.groupBoxPairs.Controls.Add(this.buttonSelectAll);
            this.groupBoxPairs.Controls.Add(this.buttonClearAll);
            this.groupBoxPairs.Controls.Add(this.comboBoxCurrencies);
            this.groupBoxPairs.Controls.Add(this.buttonPickPairs);
            this.groupBoxPairs.Location = new System.Drawing.Point(7, 36);
            this.groupBoxPairs.Name = "groupBoxPairs";
            this.groupBoxPairs.Size = new System.Drawing.Size(251, 243);
            this.groupBoxPairs.TabIndex = 73;
            this.groupBoxPairs.TabStop = false;
            this.groupBoxPairs.Text = "Select  pairs";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(728, 28);
            this.menuStrip1.TabIndex = 74;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.aboutToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(125, 26);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // buttonCsRun
            // 
            this.buttonCsRun.BackColor = System.Drawing.SystemColors.Control;
            this.buttonCsRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCsRun.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.buttonCsRun.Location = new System.Drawing.Point(0, 83);
            this.buttonCsRun.Name = "buttonCsRun";
            this.buttonCsRun.Size = new System.Drawing.Size(75, 26);
            this.buttonCsRun.TabIndex = 75;
            this.buttonCsRun.Text = "Run";
            this.buttonCsRun.UseVisualStyleBackColor = false;
            this.buttonCsRun.Click += new System.EventHandler(this.buttonCsRun_Click);
            // 
            // comboBoxExpertAttribs
            // 
            this.comboBoxExpertAttribs.FormattingEnabled = true;
            this.comboBoxExpertAttribs.Location = new System.Drawing.Point(0, 21);
            this.comboBoxExpertAttribs.Name = "comboBoxExpertAttribs";
            this.comboBoxExpertAttribs.Size = new System.Drawing.Size(199, 24);
            this.comboBoxExpertAttribs.TabIndex = 76;
            this.comboBoxExpertAttribs.Text = "<none>";
            this.comboBoxExpertAttribs.SelectedIndexChanged += new System.EventHandler(this.comboBoxExpertAttribs_SelectedIndexChanged);
            // 
            // groupBoxCs
            // 
            this.groupBoxCs.Controls.Add(this.labelCsExpertDescription);
            this.groupBoxCs.Controls.Add(this.comboBoxExpertAttribs);
            this.groupBoxCs.Controls.Add(this.buttonCsRun);
            this.groupBoxCs.Location = new System.Drawing.Point(281, 144);
            this.groupBoxCs.Name = "groupBoxCs";
            this.groupBoxCs.Size = new System.Drawing.Size(212, 115);
            this.groupBoxCs.TabIndex = 77;
            this.groupBoxCs.TabStop = false;
            this.groupBoxCs.Text = "C# expert";
            // 
            // labelCsExpertDescription
            // 
            this.labelCsExpertDescription.AutoSize = true;
            this.labelCsExpertDescription.Location = new System.Drawing.Point(6, 50);
            this.labelCsExpertDescription.Name = "labelCsExpertDescription";
            this.labelCsExpertDescription.Size = new System.Drawing.Size(54, 17);
            this.labelCsExpertDescription.TabIndex = 77;
            this.labelCsExpertDescription.Text = "<desc>";
            // 
            // groupBoxTimeframe
            // 
            this.groupBoxTimeframe.Controls.Add(this.radioButtonD1);
            this.groupBoxTimeframe.Controls.Add(this.radioButtonH4);
            this.groupBoxTimeframe.Controls.Add(this.radioButtonH1);
            this.groupBoxTimeframe.Controls.Add(this.radioButtonM30);
            this.groupBoxTimeframe.Controls.Add(this.radioButtonM15);
            this.groupBoxTimeframe.Controls.Add(this.radioButtonM5);
            this.groupBoxTimeframe.Controls.Add(this.radioButtonM1);
            this.groupBoxTimeframe.Location = new System.Drawing.Point(599, 31);
            this.groupBoxTimeframe.Name = "groupBoxTimeframe";
            this.groupBoxTimeframe.Size = new System.Drawing.Size(122, 134);
            this.groupBoxTimeframe.TabIndex = 80;
            this.groupBoxTimeframe.TabStop = false;
            this.groupBoxTimeframe.Text = "Time frame";
            // 
            // radioButtonD1
            // 
            this.radioButtonD1.AutoSize = true;
            this.radioButtonD1.Location = new System.Drawing.Point(68, 75);
            this.radioButtonD1.Name = "radioButtonD1";
            this.radioButtonD1.Size = new System.Drawing.Size(47, 21);
            this.radioButtonD1.TabIndex = 6;
            this.radioButtonD1.TabStop = true;
            this.radioButtonD1.Text = "D1";
            this.radioButtonD1.UseVisualStyleBackColor = true;
            // 
            // radioButtonH4
            // 
            this.radioButtonH4.AutoSize = true;
            this.radioButtonH4.Location = new System.Drawing.Point(68, 48);
            this.radioButtonH4.Name = "radioButtonH4";
            this.radioButtonH4.Size = new System.Drawing.Size(47, 21);
            this.radioButtonH4.TabIndex = 5;
            this.radioButtonH4.TabStop = true;
            this.radioButtonH4.Text = "H4";
            this.radioButtonH4.UseVisualStyleBackColor = true;
            // 
            // radioButtonH1
            // 
            this.radioButtonH1.AutoSize = true;
            this.radioButtonH1.Location = new System.Drawing.Point(68, 21);
            this.radioButtonH1.Name = "radioButtonH1";
            this.radioButtonH1.Size = new System.Drawing.Size(47, 21);
            this.radioButtonH1.TabIndex = 4;
            this.radioButtonH1.TabStop = true;
            this.radioButtonH1.Text = "H1";
            this.radioButtonH1.UseVisualStyleBackColor = true;
            // 
            // radioButtonM30
            // 
            this.radioButtonM30.AutoSize = true;
            this.radioButtonM30.Location = new System.Drawing.Point(5, 102);
            this.radioButtonM30.Name = "radioButtonM30";
            this.radioButtonM30.Size = new System.Drawing.Size(56, 21);
            this.radioButtonM30.TabIndex = 3;
            this.radioButtonM30.TabStop = true;
            this.radioButtonM30.Text = "m30";
            this.radioButtonM30.UseVisualStyleBackColor = true;
            // 
            // radioButtonM15
            // 
            this.radioButtonM15.AutoSize = true;
            this.radioButtonM15.Location = new System.Drawing.Point(5, 75);
            this.radioButtonM15.Name = "radioButtonM15";
            this.radioButtonM15.Size = new System.Drawing.Size(56, 21);
            this.radioButtonM15.TabIndex = 2;
            this.radioButtonM15.TabStop = true;
            this.radioButtonM15.Text = "m15";
            this.radioButtonM15.UseVisualStyleBackColor = true;
            // 
            // radioButtonM5
            // 
            this.radioButtonM5.AutoSize = true;
            this.radioButtonM5.Location = new System.Drawing.Point(5, 48);
            this.radioButtonM5.Name = "radioButtonM5";
            this.radioButtonM5.Size = new System.Drawing.Size(48, 21);
            this.radioButtonM5.TabIndex = 1;
            this.radioButtonM5.TabStop = true;
            this.radioButtonM5.Text = "m5";
            this.radioButtonM5.UseVisualStyleBackColor = true;
            // 
            // radioButtonM1
            // 
            this.radioButtonM1.AutoSize = true;
            this.radioButtonM1.Location = new System.Drawing.Point(6, 21);
            this.radioButtonM1.Name = "radioButtonM1";
            this.radioButtonM1.Size = new System.Drawing.Size(48, 21);
            this.radioButtonM1.TabIndex = 0;
            this.radioButtonM1.TabStop = true;
            this.radioButtonM1.Text = "m1\r\n";
            this.radioButtonM1.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelAccount,
            this.toolStripStatusLabelConnection});
            this.statusStrip1.Location = new System.Drawing.Point(0, 458);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(728, 25);
            this.statusStrip1.TabIndex = 81;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelAccount
            // 
            this.toolStripStatusLabelAccount.Name = "toolStripStatusLabelAccount";
            this.toolStripStatusLabelAccount.Size = new System.Drawing.Size(61, 20);
            this.toolStripStatusLabelAccount.Text = "account";
            // 
            // toolStripStatusLabelConnection
            // 
            this.toolStripStatusLabelConnection.Name = "toolStripStatusLabelConnection";
            this.toolStripStatusLabelConnection.Size = new System.Drawing.Size(82, 20);
            this.toolStripStatusLabelConnection.Text = "connection";
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 483);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonStopExperts);
            this.Controls.Add(this.groupBoxCs);
            this.Controls.Add(this.groupBoxPairs);
            this.Controls.Add(this.groupBoxJournal);
            this.Controls.Add(this.groupBoxPython);
            this.Controls.Add(this.textBoxDisplay);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBoxTimeframe);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ManagerForm";
            this.Text = "PipSeq Demo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManagerForm_FormClosing);
            this.Load += new System.EventHandler(this.ManagerForm_Load);
            this.groupBoxPython.ResumeLayout(false);
            this.groupBoxPython.PerformLayout();
            this.groupBoxJournal.ResumeLayout(false);
            this.groupBoxJournal.PerformLayout();
            this.groupBoxPairs.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBoxCs.ResumeLayout(false);
            this.groupBoxCs.PerformLayout();
            this.groupBoxTimeframe.ResumeLayout(false);
            this.groupBoxTimeframe.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonPickPairs;
        private System.Windows.Forms.ComboBox comboBoxCurrencies;
        private System.Windows.Forms.Button buttonClearAll;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.CheckedListBox checkedListBoxPairs;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button buttonPyFileDialog;
        private System.Windows.Forms.TextBox textBoxPyFile;
        private System.Windows.Forms.Button buttonPython;
        private System.Windows.Forms.OpenFileDialog openFileDialogPy;
        private System.Windows.Forms.TextBox textBoxDisplay;
        private System.Windows.Forms.GroupBox groupBoxPython;
        private System.Windows.Forms.Button buttonStopExperts;
        private System.Windows.Forms.GroupBox groupBoxJournal;
        private System.Windows.Forms.RadioButton radioButtonJournalRead;
        private System.Windows.Forms.RadioButton radioButtonJournalWrite;
        private System.Windows.Forms.RadioButton radioButtonNoJournal;
        private System.Windows.Forms.GroupBox groupBoxPairs;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.Button buttonCsRun;
        private System.Windows.Forms.ComboBox comboBoxExpertAttribs;
        private System.Windows.Forms.GroupBox groupBoxCs;
        private System.Windows.Forms.GroupBox groupBoxTimeframe;
        private System.Windows.Forms.RadioButton radioButtonD1;
        private System.Windows.Forms.RadioButton radioButtonH4;
        private System.Windows.Forms.RadioButton radioButtonH1;
        private System.Windows.Forms.RadioButton radioButtonM30;
        private System.Windows.Forms.RadioButton radioButtonM15;
        private System.Windows.Forms.RadioButton radioButtonM5;
        private System.Windows.Forms.RadioButton radioButtonM1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelAccount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelConnection;
        private System.Windows.Forms.Label labelCsExpertDescription;
    }
}
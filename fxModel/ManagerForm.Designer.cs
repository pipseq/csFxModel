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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonStopPython = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxAppend = new System.Windows.Forms.CheckBox();
            this.radioButtonJournalRead = new System.Windows.Forms.RadioButton();
            this.radioButtonJournalWrite = new System.Windows.Forms.RadioButton();
            this.radioButtonNoJournal = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            this.checkedListBoxPairs.Location = new System.Drawing.Point(102, 30);
            this.checkedListBoxPairs.Margin = new System.Windows.Forms.Padding(4);
            this.checkedListBoxPairs.Name = "checkedListBoxPairs";
            this.checkedListBoxPairs.Size = new System.Drawing.Size(159, 208);
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
            this.buttonPyFileDialog.Location = new System.Drawing.Point(154, 32);
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
            this.textBoxPyFile.Size = new System.Drawing.Size(100, 22);
            this.textBoxPyFile.TabIndex = 61;
            // 
            // buttonPython
            // 
            this.buttonPython.Enabled = false;
            this.buttonPython.Location = new System.Drawing.Point(46, 64);
            this.buttonPython.Margin = new System.Windows.Forms.Padding(4);
            this.buttonPython.Name = "buttonPython";
            this.buttonPython.Size = new System.Drawing.Size(66, 28);
            this.buttonPython.TabIndex = 60;
            this.buttonPython.Text = "Run";
            this.buttonPython.UseVisualStyleBackColor = true;
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
            this.textBoxDisplay.Location = new System.Drawing.Point(12, 301);
            this.textBoxDisplay.Multiline = true;
            this.textBoxDisplay.Name = "textBoxDisplay";
            this.textBoxDisplay.ReadOnly = true;
            this.textBoxDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDisplay.Size = new System.Drawing.Size(497, 177);
            this.textBoxDisplay.TabIndex = 64;
            this.textBoxDisplay.WordWrap = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonStopPython);
            this.groupBox1.Controls.Add(this.textBoxPyFile);
            this.groupBox1.Controls.Add(this.buttonPython);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.buttonPyFileDialog);
            this.groupBox1.Location = new System.Drawing.Point(293, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 140);
            this.groupBox1.TabIndex = 65;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Python expert";
            // 
            // buttonStopPython
            // 
            this.buttonStopPython.Enabled = false;
            this.buttonStopPython.Location = new System.Drawing.Point(46, 99);
            this.buttonStopPython.Name = "buttonStopPython";
            this.buttonStopPython.Size = new System.Drawing.Size(66, 30);
            this.buttonStopPython.TabIndex = 64;
            this.buttonStopPython.Text = "Stop";
            this.buttonStopPython.UseVisualStyleBackColor = true;
            this.buttonStopPython.Click += new System.EventHandler(this.buttonStopPython_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxAppend);
            this.groupBox2.Controls.Add(this.radioButtonJournalRead);
            this.groupBox2.Controls.Add(this.radioButtonJournalWrite);
            this.groupBox2.Controls.Add(this.radioButtonNoJournal);
            this.groupBox2.Location = new System.Drawing.Point(293, 182);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(156, 113);
            this.groupBox2.TabIndex = 72;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Journal";
            // 
            // checkBoxAppend
            // 
            this.checkBoxAppend.AutoSize = true;
            this.checkBoxAppend.Enabled = false;
            this.checkBoxAppend.Location = new System.Drawing.Point(74, 86);
            this.checkBoxAppend.Name = "checkBoxAppend";
            this.checkBoxAppend.Size = new System.Drawing.Size(79, 21);
            this.checkBoxAppend.TabIndex = 71;
            this.checkBoxAppend.Text = "Append";
            this.checkBoxAppend.UseVisualStyleBackColor = true;
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
            this.radioButtonNoJournal.Location = new System.Drawing.Point(6, 33);
            this.radioButtonNoJournal.Name = "radioButtonNoJournal";
            this.radioButtonNoJournal.Size = new System.Drawing.Size(63, 21);
            this.radioButtonNoJournal.TabIndex = 68;
            this.radioButtonNoJournal.TabStop = true;
            this.radioButtonNoJournal.Text = "None";
            this.radioButtonNoJournal.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkedListBoxPairs);
            this.groupBox3.Controls.Add(this.buttonSelectAll);
            this.groupBox3.Controls.Add(this.buttonClearAll);
            this.groupBox3.Controls.Add(this.comboBoxCurrencies);
            this.groupBox3.Controls.Add(this.buttonPickPairs);
            this.groupBox3.Location = new System.Drawing.Point(7, 36);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(280, 259);
            this.groupBox3.TabIndex = 73;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Select  pairs";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(515, 28);
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
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(181, 26);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 483);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxDisplay);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ManagerForm";
            this.Text = "PipSeq Demo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManagerForm_FormClosing);
            this.Load += new System.EventHandler(this.ManagerForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonStopPython;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonJournalRead;
        private System.Windows.Forms.RadioButton radioButtonJournalWrite;
        private System.Windows.Forms.RadioButton radioButtonNoJournal;
        private System.Windows.Forms.CheckBox checkBoxAppend;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
    }
}
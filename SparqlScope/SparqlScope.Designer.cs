namespace sparqlScope
{
    partial class SparqlScope
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
            this.buttonRun = new System.Windows.Forms.Button();
            this.textBoxResults = new System.Windows.Forms.TextBox();
            this.textBoxQuery = new System.Windows.Forms.TextBox();
            this.checkBoxUseLongNames = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.textBoxNodeDescription = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(13, 13);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 0;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // textBoxResults
            // 
            this.textBoxResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResults.Location = new System.Drawing.Point(3, 3);
            this.textBoxResults.Multiline = true;
            this.textBoxResults.Name = "textBoxResults";
            this.textBoxResults.ReadOnly = true;
            this.textBoxResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxResults.Size = new System.Drawing.Size(358, 260);
            this.textBoxResults.TabIndex = 1;
            // 
            // textBoxQuery
            // 
            this.textBoxQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxQuery.Location = new System.Drawing.Point(3, 3);
            this.textBoxQuery.Multiline = true;
            this.textBoxQuery.Name = "textBoxQuery";
            this.textBoxQuery.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxQuery.Size = new System.Drawing.Size(358, 264);
            this.textBoxQuery.TabIndex = 2;
            this.textBoxQuery.TabStop = false;
            // 
            // checkBoxUseLongNames
            // 
            this.checkBoxUseLongNames.AutoSize = true;
            this.checkBoxUseLongNames.Location = new System.Drawing.Point(107, 15);
            this.checkBoxUseLongNames.Name = "checkBoxUseLongNames";
            this.checkBoxUseLongNames.Size = new System.Drawing.Size(132, 21);
            this.checkBoxUseLongNames.TabIndex = 3;
            this.checkBoxUseLongNames.Text = "Use long names";
            this.checkBoxUseLongNames.UseVisualStyleBackColor = true;
            this.checkBoxUseLongNames.CheckedChanged += new System.EventHandler(this.checkBoxUseLongNames_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(268, 50);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBoxQuery);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBoxResults);
            this.splitContainer1.Size = new System.Drawing.Size(364, 540);
            this.splitContainer1.SplitterDistance = 270;
            this.splitContainer1.TabIndex = 4;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(12, 53);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(250, 412);
            this.treeView1.TabIndex = 3;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            // 
            // textBoxNodeDescription
            // 
            this.textBoxNodeDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxNodeDescription.Location = new System.Drawing.Point(12, 471);
            this.textBoxNodeDescription.Multiline = true;
            this.textBoxNodeDescription.Name = "textBoxNodeDescription";
            this.textBoxNodeDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxNodeDescription.Size = new System.Drawing.Size(250, 119);
            this.textBoxNodeDescription.TabIndex = 2;
            // 
            // SparqlScope
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 602);
            this.Controls.Add(this.textBoxNodeDescription);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.checkBoxUseLongNames);
            this.Controls.Add(this.buttonRun);
            this.Name = "SparqlScope";
            this.Text = "SPARQL Scope";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SparqlScope_FormClosing);
            this.Load += new System.EventHandler(this.SparqlScope_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.TextBox textBoxResults;
        private System.Windows.Forms.TextBox textBoxQuery;
        private System.Windows.Forms.CheckBox checkBoxUseLongNames;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TextBox textBoxNodeDescription;
    }
}


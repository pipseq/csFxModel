using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.hp.hpl.jena.rdf.model;
using org.pipseq.rdf.jena.util;
using org.pipseq.rdf.jena.cfg;
namespace sparqlScope
{

    /**
    Needs a directory structure for the modelwrappers with access to their models.
    */
    public partial class SparqlScope : Form
    {
        Model model;
        SparqlScopeImpl scope;
        public SparqlScope()
        {
            InitializeComponent();
            java.util.Map map = WrapperRegistry.getInstance().getMap();
            for (java.util.Iterator it = map.keySet().iterator();it.hasNext();)
            {
                string key = (string)it.next();
                ModelWrapper mw = (ModelWrapper)map.get(key);
                if (mw is BoxWrapper){
                    ModelWrapper abox = ((BoxWrapper)mw).getAbox();
                    ModelWrapper tbox = ((BoxWrapper)mw).getTbox();
                    TreeNode tn2 = new TreeNode(abox.getModelName());
                    TreeNode tn3 = new TreeNode(tbox.getModelName());
                    TreeNode[] tna = new TreeNode[] { tn2, tn3 };
                    tn2.Tag = abox;
                    tn3.Tag = tbox;
                    TreeNode tn = new TreeNode(key,tna);
                    tn.Tag = mw;
                    this.treeView1.Nodes.Add(tn);
                } else
                {
                    TreeNode tn = new TreeNode(key);
                    tn.Tag = mw;
                    this.treeView1.Nodes.Add(tn);
                }
            }
        }

        public SparqlScope(object model):this()
        {
            initModel((Model)model);
        }

        private void initModel(Model model)
        {
            this.model = (Model)model;
            scope = new SparqlScopeImpl(this.model);
            scope.setUseShortNames(true);

        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            string selected = this.textBoxQuery.SelectedText;
            if (selected.Trim() == "")
                selected = this.textBoxQuery.Text;
            query(selected);
        }

        private void query(string query)
        {
            string results = scope.submitCommands(query);
            string normalResults = results.Replace("\n", "\r\n");
            this.textBoxResults.Text = normalResults;
        }

        private void checkBoxUseLongNames_CheckedChanged(object sender, EventArgs e)
        {
            scope.setUseShortNames(!this.checkBoxUseLongNames.Checked);
        }

        private string scopeFile = @"sparqlScope.txt";
        private void SparqlScope_Load(object sender, EventArgs e)
        {
            if (File.Exists(scopeFile))
            using (StreamReader sr = new StreamReader(scopeFile))
            {
                this.textBoxQuery.Text = sr.ReadToEnd();
            }
        }

        private void SparqlScope_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(scopeFile))
            {
                string sql = this.textBoxQuery.Text;
                sw.Write(sql);
                sw.Flush();
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ModelWrapper mw = (ModelWrapper)e.Node.Tag;
            this.textBoxNodeDescription.Text = mw.toString();

        }
        TreeNode previousNode;
        Color previousColor;
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode tn = e.Node;
            ModelWrapper mw = (ModelWrapper)tn.Tag;
            this.textBoxNodeDescription.Text = mw.toString();
            initModel(mw.get());

            if (previousNode != null)
            {
                previousNode.BackColor = previousColor;
            }
            previousColor = tn.BackColor;
            tn.BackColor = Color.LightSteelBlue;
            previousNode = tn;
        }
    }
}

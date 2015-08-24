using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common
{
    public partial class TestMailSenderForm1 : Form
    {
        public TestMailSenderForm1()
        {
            InitializeComponent();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string code = "Gnarly";
            string symbol = this.textBoxSym.Text;
            DateTime stop = DateTime.Now;
            string last = this.textBoxLast.Text;
            string result = this.textBoxDir.Text;

            alert(code, symbol,
                string.Format(
                "{0};{1};{2};{3};{4};{5};{6}",
                stop, symbol, last,
                result, last, last, "50000"));

        }

        MailSender mailSender = new MailSender();

        public void alert(string code, string symbol, string description)
        {
            if (mailSender != null)
            {
                mailSender.sendMail(code, description);
                //log.info("{0}:{1}:{2}", code, symbol, description);
            }
        }

    }
}

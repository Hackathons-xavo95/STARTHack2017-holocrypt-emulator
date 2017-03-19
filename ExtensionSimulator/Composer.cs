using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtensionSimulator
{
    public partial class Composer : Form
    {
        private string subject, message;

        public Composer()
        {
            InitializeComponent();
        }

        public string getSubject() { return this.subject; }

        public string getMessage() { return this.message; }

        private void button1_Click(object sender, EventArgs e)
        {
            this.subject = textBox1.Text;
            this.message = textBox2.Text;
            if (this.subject!= "" && this.message != "")
                this.DialogResult = DialogResult.OK;
        }
    }
}

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
    public partial class UserForm : Form
    {
        private string user, pass;

        public UserForm()
        {
            InitializeComponent();
        }

        public string getUser() { return this.user; }

        public string getPass() { return this.pass; }

        private void button1_Click(object sender, EventArgs e)
        {
            this.user = textBox1.Text;
            this.pass = textBox2.Text;
            if (this.user != "" && this.pass != "")
                this.DialogResult = DialogResult.OK;
        }
    }
}

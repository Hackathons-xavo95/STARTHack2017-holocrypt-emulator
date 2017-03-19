using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Flurl.Http;
using QRCoder;

namespace ExtensionSimulator
{
    public partial class MessageViewer : Form
    {
        private string validationUsername, validationPassword;
        SqlConnection cnn;
        private List<Bitmap> qrList;
        private List<List<String>> messages;
        private bool safe;

        private void SetUpConnection() {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Database.mdf;Integrated Security=True";
            cnn = new SqlConnection(connectionString);
            try
            {
                cnn.Open();
                cnn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! - Exception: " + ex);
            }
        }

        private void InsertEmail(string newId, string newSubject, string newMessage)
        {
            cnn.Open();
            string sql = "INSERT INTO messages (Id, Subject, Message) VALUES ('" + newId + "', '" + newSubject + "', '" + newMessage + "')";
            try
            {
                SqlCommand command = new SqlCommand(sql, cnn);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! - Exception: " + ex);
            }
        }

        private List<List<String>> GetEmails()
        {
            string sql = "SELECT * FROM messages";
            try
            {
                SqlCommand command = new SqlCommand(sql, cnn);
                SqlDataReader dataReader = command.ExecuteReader();
                List<List<String>> result = new List<List<string>>();
                while (dataReader.Read())
                {
                    List<String> email = new List<string>();
                    email.Add(Convert.ToString(dataReader.GetValue(0)));
                    email.Add(Convert.ToString(dataReader.GetValue(1)));
                    string message = Convert.ToString(dataReader.GetValue(2)).Replace(@"\n", Environment.NewLine);
                    email.Add(message);
                    result.Add(email);
                }
                dataReader.Close();
                command.Dispose();
                cnn.Close();
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! - Exception: " + ex);
                return null;
            }
        }

        private void Initialize()
        {
            InitializeComponent();
            this.listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            SetUpConnection();
            cnn.Open();
            messages = GetEmails();
            if (!this.safe)
            {
                qrList = new List<Bitmap>();
                for(int i = 0; i < messages.Count; i++)
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(messages[i][0], QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    qrList.Add(qrCodeImage);

                    Petition(i);
                }
            }
            FillList();
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex > -1)
            {
                if (safe)
                    this.textBox1.Text = this.messages[this.listBox1.SelectedIndex][2];
                else
                    this.pictureBox1.Image = this.qrList[this.listBox1.SelectedIndex];
            }
        }

        private async Task Petition(int i)
        {
            var responseString = await "https://holocrypt.herokuapp.com/api/messages/post"
                    .PostUrlEncodedAsync(new { id = messages[i][0], username = validationUsername, password = validationPassword, message = messages[i][2] })
                    .ReceiveString();
            messages[i][2] = "";
        }
        private void FillList()
        {
            for (int i = 0; i < messages.Count; i++)
                this.listBox1.Items.Add(messages[i][1]);
        }

        public MessageViewer()
        {
            safe = true;
            Initialize();
            this.textBox1.Visible = true;
            this.pictureBox1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Composer messenger = new Composer();
            if(messenger.ShowDialog() == DialogResult.OK)
            {
                InsertEmail(Convert.ToString(messages.Count), messenger.getSubject(), messenger.getMessage());
                List<String> email = new List<string>();
                email.Add(Convert.ToString(messages.Count));
                email.Add(messenger.getSubject());
                //string message = Convert.ToString(dataReader.GetValue(2)).Replace(@"\n", Environment.NewLine);
                email.Add(messenger.getMessage());
                messages.Add(email);
                if(!safe)
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(email[0], QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    qrList.Add(qrCodeImage);

                    Petition(Convert.ToInt32(email[0]));
                }
                this.listBox1.Items.Clear();
                FillList();
            } 
        }

        public MessageViewer(string pUsername, string pPassword)
        {
            safe = false;
            this.validationUsername = pUsername;
            this.validationPassword = pPassword;
            Initialize();
            this.textBox1.Visible = false;
            this.pictureBox1.Visible = true;
        }
    }
}

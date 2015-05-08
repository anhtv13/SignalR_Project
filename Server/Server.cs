using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignalRForm
{
    public partial class Server : Form
    {
        string url = "http://localhost:1234";

        public Server()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                WebApp.Start(url);
                richTextBox1.AppendText("Server starts at " + url);
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex + "\n Fail to start server.");
            }
        }
    }
}

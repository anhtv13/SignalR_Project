using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Client;
using Model.Model;
using Client;


namespace Client
{
    public partial class ChatForm : Form
    {
        public IHubProxy HubProxy;
        //public List<UserPerson> listUser;
        private Messages m_message;
        public string senderId;
        public string receiverId;
        public string ownerName;
        private string m_content;
        private DateTime m_dt;

        public ChatForm(IHubProxy hubProxy)
        {
            InitializeComponent();
            HubProxy = hubProxy;
            //HubProxy.On<Messages>("SendMessageToClient", (message) => Invoke((Action)(() => AppearMessage(message))));
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {           
            m_content = textBoxMessage.Text;
            m_dt = DateTime.Now;
            m_message = new Messages(senderId, receiverId, m_content, m_dt);
            richTextBoxMessage.AppendText(ownerName + ": " + m_content + "\n");
            HubProxy.Invoke<Messages>("SendMessageToServer", m_message);
            textBoxMessage.Text = "";
        }

        public void showMessage(string message)
        {
            richTextBoxMessage.AppendText(message);
        }
    }
}

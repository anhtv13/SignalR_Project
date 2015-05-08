using Microsoft.AspNet.SignalR.Client;
using Model.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Client : Form
    {
        public IHubProxy HubProxy { get; set; }
        const string ServerURI = "http://localhost:1234";
        private HubConnection Connection { get; set; }

        private string m_userName;
        private string m_passWord;
        private UserPerson up;
        private Dictionary<string, UserPerson> m_UserDictionary;
        private Dictionary<string, ChatForm> m_ChatFormDictionary;

        public Client()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_userName = textBoxUserName.Text;
            m_passWord = textBoxPassWord.Text;

            ConnectAsync();
        }

        private void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI);
            m_ChatFormDictionary = new Dictionary<string, ChatForm>();
            m_UserDictionary = new Dictionary<string, UserPerson>();
            HubProxy = Connection.CreateHubProxy("MyHub");

            HubProxy.On<string, char>("BroadcastOnlineStatus", (displayName, status) => Invoke((Action)(() => ChangeStatus(displayName, status))));
            HubProxy.On<Messages>("SendMessageToClient", (message) => Invoke((Action)(() => AppearMessage(message))));
            HubProxy.On<List<Messages>>("SendOfflineMessage", (offlineMessage) => Invoke((Action)(() => AppearOfflineMessage(offlineMessage))));
            


            try
            {
                Connection.Start().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        //Connection error                        
                        Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show(task.Exception.Message);
                        });
                    }
                    else
                    {
                        //connected
                        Invoke((MethodInvoker)delegate
                        {
                            label3.Text = "Connected to server at " + ServerURI +"";                              
                            up = new UserPerson(m_userName, m_passWord) { Status='O'};
                            getUserList();
                            bool m_result = HubProxy.Invoke<bool>("Login", up).Result;
                            string connectionId = HubProxy.Invoke<string>("GetConnectionId").Result;
                            if (m_result == false)
                            {
                                MessageBox.Show("Log in failed.");
                            }
                            else
                            {
                                //MessageBox.Show(connectionId); 
                                
                                showUserList();
                                buttonLogin.Enabled = false;           
                            }
                        });


                    }
                });
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// appear OfflineMessage
        /// </summary>
        /// <param name="offlineMessage"></param>
        private void AppearOfflineMessage(List<Messages> offlineMessage)
        {
            //string s="";
            //for (int i = 0; i < offlineMessage.Count; i++)
            //{
            //    s = s + offlineMessage[i].Sender + ": " + offlineMessage[i].Content + "\n";
            //}
            //MessageBox.Show(s);
            foreach (Messages m in offlineMessage)
            {
                AppearMessage(m);
            }
        }

        /// <summary>
        /// appear Message to ChatForm
        /// </summary>
        /// <param name="message"></param>
        private void AppearMessage(Messages message)
        {
            string senderName = m_UserDictionary[message.Sender].DisplayName;
            string receiverName = m_UserDictionary[message.Receiver].DisplayName;
           
            if (m_ChatFormDictionary.ContainsKey(message.Sender))
            {
                string s = senderName + ": " + message.Content + "\n";
                if (m_ChatFormDictionary[message.Sender].IsDisposed)
                {
                    m_ChatFormDictionary[message.Sender]= CreateChatForm(2, message.Sender, message.Receiver);
                }
                m_ChatFormDictionary[message.Sender].showMessage(s);
            }

            else
            {
                string s = senderName + ": " + message.Content + "\n";
                ChatForm c = CreateChatForm(2, message.Sender, message.Receiver);
                c.showMessage(s);
                m_ChatFormDictionary.Add(message.Sender, c);
            }
            
        }

        /// <summary>
        /// change status of other users in ListView
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="status"></param>
        private void ChangeStatus(string userName, char status)
        {
            foreach (ListViewItem item in listViewUser.Items)
            {
                if (item.SubItems[0].Text.Equals(userName))
                {
                    item.SubItems[1].Text = ConvertStatus(status);
                }
            }           
        }

        /// <summary>
        /// get the UserList to dictionary
        /// </summary>
        private void getUserList()
        {
            List<UserPerson> listUser = HubProxy.Invoke<List<UserPerson>>("GetUserList").Result;
            
            for (int i = 0; i < listUser.Count; i++)
            {
                m_UserDictionary.Add(listUser[i].Id, listUser[i]);
            }              
        }

        /// <summary>
        /// show UserList to ListView
        /// </summary>
        private void showUserList()
        {
            foreach (UserPerson userPerson in m_UserDictionary.Values)
            {
                if (userPerson.Id != up.Id)
                {
                    ListViewItem ListItem = new ListViewItem(userPerson.DisplayName);
                    ListItem.SubItems.Add(ConvertStatus(userPerson.Status));
                    ListItem.SubItems.Add(userPerson.Id);
                    ListItem.Tag = userPerson;
                    listViewUser.Items.Add(ListItem);
                }
            }
        }

        /// <summary>
        /// convert status char into String
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public string ConvertStatus(char c)
        {
            String s="Offline";
            if (c == 'O')
            {
                s = "Online";
            }            
            return s;
        }
       
        private void chatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String receiverId = listViewUser.SelectedItems[0].SubItems[2].Text;
            //MessageBox.Show(nameChat);
            if (m_ChatFormDictionary.ContainsKey(receiverId) == false)
            {
                ChatForm c = CreateChatForm(1, up.Id, receiverId);
                m_ChatFormDictionary.Add(receiverId, c);
            }         
        }

        /// <summary>
        /// create a chat form
        /// </summary>
        /// <param name="type">type1: create form by sender, other: create form by receiver</param>
        /// <param name="senderId">SenderId</param>
        /// <param name="receiverId">ReceiverId</param>
        /// <returns>ChatForm</returns>
        public ChatForm CreateChatForm(int type, string senderId, string receiverId)
        {
            string senderName = m_UserDictionary[senderId].DisplayName;
            string receiverName = m_UserDictionary[receiverId].DisplayName;
            
            ChatForm c = new ChatForm(HubProxy);
            //set value for ChatForm
            if (type == 1)
            {
                c.Text = receiverName; 
                c.senderId = senderId;
                c.receiverId = receiverId;
                c.ownerName = senderName;
            }
            else
            {
                c.Text = senderName;
                c.senderId = receiverId;
                c.receiverId = senderId;
                c.ownerName = receiverName;
            }
            
            
            c.Show();
            

            return c;
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listViewUser.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            } 
        }


    }
}

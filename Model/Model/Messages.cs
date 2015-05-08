using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class Messages
    {
        private String m_id;
        private String m_senderId;
        private String m_receiverId;
        private String m_content;
        private DateTime m_datetime;
        private bool isRead;

        public bool IsRead
        {
            get { return isRead; }
            set { isRead = value; }
        }

        public String Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public String Sender
        {
            get { return m_senderId; }
            set { m_senderId = value; }
        }

        public String Receiver
        {
            get { return m_receiverId; }
            set { m_receiverId = value; }
        }        

        public String Content
        {
            get { return m_content; }
            set { m_content = value; }
        }

        public DateTime Datetime
        {
            get { return m_datetime; }
            set { m_datetime = value; }
        }

        public String toString()
        {
            return m_senderId + ", " + m_receiverId + ", " + m_content + ", " + m_datetime;
        }

        public Messages() { }

        public Messages(String senderId, String receiverId, String content, DateTime datetime)
        {
            this.m_senderId = senderId;
            this.m_receiverId = receiverId;
            this.m_content = content;
            this.m_datetime = datetime;
        }
        

    }
}

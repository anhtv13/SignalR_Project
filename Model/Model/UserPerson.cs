using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class UserPerson
    {
        private string m_connectionId;
        private string m_id;
        private string m_password;
        private string m_displayName;
        private char m_status; // A(Available), I(Invisible), O(Offline), etc
        private char m_role; //A(Admin) , M(Member), etc
        private List<string> m_friendList; // List contains FriendId
        
        public List<string> FriendList
        {
            get { return m_friendList; }
            set { m_friendList = value; }
        }

        public string ConnectionId
        {
            get { return m_connectionId; }
            set { m_connectionId = value; }
        }

        public string Id
        {
            get { return m_id; }
            set { m_id = value; }
        }      

        public string Password
        {
            get { return m_password; }
            set { m_password = value; }
        }
        
        public string DisplayName
        {
            get { return m_displayName; }
            set { m_displayName = value; }
        }       

        public char Status
        {
            get { return m_status; }
            set { m_status = value; }
        }
        
        public char Role
        {
            get { return m_role; }
            set { m_role = value; }
        }
        public UserPerson() { }

        public UserPerson(string id, string password)
        {
            this.m_id = id;
            this.m_password = password;
        }

        public UserPerson(string id, string password, string displayName, char status, char role)
        {       
            this.m_id = id;          
            this.m_password = password;
            this.m_displayName = displayName;
            this.m_status = status;
            this.m_role = role;
            m_friendList = new List<string>();
        }

        public void AddFriend(string friendId)
        {
            if(!m_friendList.Contains(friendId)){
                m_friendList.Add(friendId);
            }
        }

        public void UnFriend(string friendId)
        {
            m_friendList.Remove(friendId);
        }

        public int CountDictionary()
        {
            return m_friendList.Count;
        }

    }
}

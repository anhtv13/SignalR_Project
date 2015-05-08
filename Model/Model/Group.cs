using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class Group
    {
        private string m_id;
        private string m_name;//name of group
        private string m_description;//description of the group
        private bool m_isPrivate; //private or public
        private List<string> m_memberList; //List contains member         
        private List<string> m_adminList;//List contains admin

        public List<string> AdminList
        {
            get { return m_adminList; }
            set { m_adminList = value; }
        }

        public List<string> MemberList
        {
            get { return m_memberList; }
            set { m_memberList = value; }
        }

        public string Id
        {
            get { return m_id; }
            set { m_id = value; }
        }
        
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }      

        public bool IsPrivate
        {
            get { return m_isPrivate; }
            set { m_isPrivate = value; }
        }

        public Group()
        {

        }

        public Group(string name, string description, bool isPrivate)
        {           
            //auto generate Id
            this.m_name = name;
            this.m_description = description;          
            this.m_isPrivate = isPrivate;
            this.m_memberList = new List<string>();
            this.m_adminList = new List<string>();
        }

        public Group(string id, string name, string description, bool isPrivate)
        {
            this.m_id = id;
            this.m_name = name;
            this.m_description = description;
            this.m_isPrivate = isPrivate;
            this.m_memberList = new List<string>();
            this.m_adminList = new List<string>();
        }

        public void addAdmin(string userId)
        {
            if (m_memberList.Contains(userId))
            {
                if(!m_adminList.Contains(userId)){
                    m_adminList.Add(userId);
                }
            }
        }

        public void RemoveAdmin(string userId)
        {
            m_adminList.Remove(userId);
        }

        public int CountAdminList()
        {
            return m_adminList.Count;
        }

        public void AddMember(string userId){
            if (!m_memberList.Contains(userId))
            {
                m_memberList.Add(userId);
            }
        }

        public void RemoveMember(string userId)
        {
            m_memberList.Remove(userId);
        }

        public int CountMemberList()
        {
            return m_memberList.Count;
        }

    }
}

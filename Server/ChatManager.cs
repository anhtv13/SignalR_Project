using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRForm
{
    public class ChatManager
    {
        private static ChatManager m_instance;
        public static ChatManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ChatManager();
                return m_instance;
            }
        }

        //user dictionary
        private Dictionary<string, UserPerson> m_userDictionary;
        //Group dictionary 
        private Dictionary<string, Group> m_groupDictionary;
        //Offline Message List
        private List<Messages> m_OfflineMessageList;
        //MessageList (contain both online & offline)
        private List<Messages> m_MessageList;

        private MyHub m_myHub;

        public ChatManager()
        {
            m_groupDictionary = new Dictionary<string, Group>();
            m_userDictionary = new Dictionary<string, UserPerson>();
            m_OfflineMessageList = new List<Messages>();
            m_MessageList = new List<Messages>();

            InitUserDict();//load user to the system
        }

        public void SetMyHub(MyHub myHub)
        {
            m_myHub = myHub;
        }

        #region Database
        /**
         * @overview Add toan bo user co trong he thong
         */
        private void InitUserDict()
        {
            UserPerson u1 = new UserPerson("1", "123") { DisplayName = "Nguyen Van A", Status = 'F' };
            UserPerson u2 = new UserPerson("2", "123") { DisplayName = "Tran Van B", Status = 'F' };
            UserPerson u3 = new UserPerson("3", "123") { DisplayName = "Pham Van C", Status = 'F' };
            UserPerson u4 = new UserPerson("4", "123") { DisplayName = "Trinh Van D", Status = 'F' };

            m_userDictionary.Add(u1.Id, u1);
            m_userDictionary.Add(u2.Id, u2);
            m_userDictionary.Add(u3.Id, u3);
            m_userDictionary.Add(u4.Id, u4);
        }
        #endregion

        #region Login/Logout
        /**
         * @overview: Login to the system
         * @effects:
         * 1. Check username and password match
         * 2. Set session
         * 3. Set status
         */
        public bool Login(UserPerson user)
        {
            if (m_userDictionary.ContainsKey(user.Id))
            {
                if (m_userDictionary[user.Id].Password == user.Password)
                {
                    //set session for user Id
                    m_userDictionary[user.Id].ConnectionId = m_myHub.GetConnectionId();
                    user.ConnectionId = m_userDictionary[user.Id].ConnectionId;

                    m_userDictionary[user.Id].Status = user.Status;
                    if (user.Status == 'O')
                    {
                        SendOfflineMessage(user.Id);
                        m_myHub.BroadcastOnlineStatus(m_userDictionary[user.Id].DisplayName, m_userDictionary[user.Id].Status, m_userDictionary[user.Id].ConnectionId);
                        
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region GetList
        /**
         * @overview: return the user list of the system
         */
        public List<UserPerson> GetUserList()
        {
            return m_userDictionary.Values.ToList();
        }

        /**
         * @overview: return the friend list(contains User Object, not User Id) of specific User
         */
        public List<UserPerson> GetFriendList(string userId)
        {
            //get the List that contains FriendId
            List<string> friendList = m_userDictionary[userId].FriendList;

            //create a new List that contains Friend Object
            List<UserPerson> retVal = new List<UserPerson>();
            foreach (string key in friendList)
            {
                //retVal List adds users based on their id
                retVal.Add(m_userDictionary[key]);
            }

            return retVal;
        }

        /**
         * @overview: return the memberList of specific group
         */
        public List<UserPerson> GetMemberList(string groupId)
        {
            //create the List that contains memberId
            List<string> memberList = m_groupDictionary[groupId].MemberList;

            //create the List that contains memberObject
            List<UserPerson> retVal = new List<UserPerson>();
            foreach (string key in memberList)
            {
                retVal.Add(m_userDictionary[key]);
            }
            return retVal;
        }

        #endregion

        #region Group
        public void CreateGroup(string userId, Group g)
        {
            //add a new group to dictionary
            m_groupDictionary.Add(g.Id, g);
            //group has a creator as a member
            g.AddMember(userId);
            //the creator is also the admin
            AddAdmin(userId, g.Id);
            //m_myHub.SendMessage(g.Name, "created group");
        }

        public void DeleteGroup(string groupId)
        {
            //remove current group from Dictionary
            Group g = m_groupDictionary[groupId];
            m_groupDictionary.Remove(groupId);
            //m_myHub.SendMessage(g.Name, "deleted group");
        }

        public Group ViewGroup(string groupId)
        {
            Group g = m_groupDictionary[groupId];
            return g;
        }

        public Group EditGroup(string groupId, string name, string description, bool isPrivate)
        {
            Group g = m_groupDictionary[groupId];
            g.Name = name;
            g.Description = description;
            g.IsPrivate = isPrivate;
            return g;
        }

        public Group SearchGroup(string groupId)
        {
            if (m_groupDictionary.ContainsKey(groupId))
            {
                return m_groupDictionary[groupId];
            }
            else
            {
                return null;
            }
        }

        public void AddMember(string userId, string groupId)
        {
            Group g = m_groupDictionary[groupId];
            UserPerson u = m_userDictionary[userId];
            g.AddMember(userId);
            //gui thong tin
            //m_myHub.SendMessage(g.Name, "added member " + u.DisplayName);

        }

        public void RemoveMember(string userId, string groupId)
        {
            Group g = m_groupDictionary[groupId];
            UserPerson u = m_userDictionary[userId];
            g.RemoveMember(userId);
            //m_myHub.SendMessage(g.Name, "removed member " + u.DisplayName);
        }


        public void AddAdmin(string userId, string groupId)
        {
            Group g = m_groupDictionary[groupId];
            UserPerson u = m_userDictionary[userId];
            g.addAdmin(userId);
            //m_myHub.SendMessage(g.Name, "added admin " + u.DisplayName);
        }

        public void RemoveAdmin(string userId, string groupId)
        {
            Group g = m_groupDictionary[groupId];
            UserPerson u = m_userDictionary[userId];
            g.RemoveAdmin(userId);
            //m_myHub.SendMessage(g.Name, "removed admin " + u.DisplayName);
        }

        #endregion

        #region Friend
        public void AddFriend(string userId, string friendId)
        {
            UserPerson user1 = m_userDictionary[userId];
            UserPerson user2 = m_userDictionary[friendId];
            user1.AddFriend(friendId);
            user2.AddFriend(userId);
        }

        public void UnFriend(string userId, string friendId)
        {
            UserPerson user1 = m_userDictionary[userId];
            UserPerson user2 = m_userDictionary[friendId];
            user1.UnFriend(friendId);
            user2.UnFriend(userId);
        }

        public List<UserPerson> SearchFriend(string friendId)
        {
            List<UserPerson> friendList;
            if (m_userDictionary.ContainsKey(friendId))
            {
                friendList = m_userDictionary.Values.Where(t => t.DisplayName.Contains(friendId)).ToList();
                return friendList;
            }
            return null;
        }
        #endregion

        #region Message
        /// <summary>
        /// send message Object to server
        /// </summary>
        /// <param name="m"></param>
        public void SendMessageToServer(Messages m)
        {
            //nếu receiver online thì gửi tin nhắn trực tiếp cho receiver
            if (m_userDictionary[m.Receiver].Status == 'O' || m_userDictionary[m.Receiver].Status == 'I')
            {
            string connectionId1 = m_userDictionary[m.Sender].ConnectionId;
            string connectionId2 = m_userDictionary[m.Receiver].ConnectionId;
            m.Datetime = DateTime.Now;//set time as same as server
            
            m_myHub.SendMessageToClient(connectionId2, m);           
            }
            
            //nếu receiver offline thì lưu tin nhắn vào OfflineMessageList
            else
            {
                m_OfflineMessageList.Add(m);
            }

            //add message to MessageList
            m_MessageList.Add(m);
        }

        public void SendOfflineMessage(string userId)
        {
            string connectionId = m_userDictionary[userId].ConnectionId;
            List<Messages> offlineMessage = new List<Messages>();

            for (int i = 0; i < m_OfflineMessageList.Count; i++)
            {            
                if (m_OfflineMessageList[i].Receiver == userId)
                {
                    offlineMessage.Add(m_OfflineMessageList[i]);//lưu tin nhắn vào list để gửi về client                    
                }
            }

            //if offline message >0, send to client
            if (offlineMessage.Count > 0)
            {
                m_myHub.SendOfflineMessage(connectionId, offlineMessage);
            }

            //remove offline message from OffLineMessageList
            m_OfflineMessageList.RemoveAll(x => x.Receiver == userId);
            
        }

        #endregion
    }
}

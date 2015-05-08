using Microsoft.AspNet.SignalR;
using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRForm
{
    public class MyHub : Hub
    {
        ChatManager m_chatManager = ChatManager.Instance;

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public MyHub()
        {
            m_chatManager.SetMyHub(this);
        }        

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        #region Login/ Logout

        /**
         * @overview: login to the chat system
         */
        public bool Login(UserPerson user)
        {
            return m_chatManager.Login(user);
        }

        /**
         * @overview: logout of the chat system
         */
        public void Logout(UserPerson up)
        {

        }

        #endregion

        #region Group

        public void CreateGroup(string userId, Group group)
        {
            //Add group add userId to group
            m_chatManager.CreateGroup(userId, group);
        }

        /**
         * @overview: delete the group
         */
        public void DeleteGroup(string groupId)
        {
            m_chatManager.DeleteGroup(groupId);
        }

        /**
         * @overview: edit info of group
         */
        public void EditGroup(string groupId, string name, string description, bool isPrivate)
        {
            m_chatManager.EditGroup(groupId, name, description, isPrivate);
        }

        /**
         * @overview: view group info
         */
        public void ViewGroup(string groupId)
        {
            m_chatManager.ViewGroup(groupId);
        }

        /**
         * @overview: add member to a group
         */
        public void AddMember(string userId, string groupId)
        {
            m_chatManager.AddMember(userId, groupId);
        }

        /**
         * @overview: delete member from a group
         */
        public void DeleteMember(string userId, string groupId)
        {
            m_chatManager.RemoveMember(userId, groupId);
        }

        /**
         * @overview: set admin for a group
         */
        public void SetAdmin(string userId, string groupId)
        {
            m_chatManager.AddAdmin(userId, groupId);
        }

        #endregion

        #region Friend
        /**
         * @overview: add friend to the list
         */
        public void AddFriend(string userId, string friendId)
        {
            m_chatManager.AddFriend(userId, friendId);
        }


        /**
         * @overview: unfriend from the list
         */
        public void UnFriend(string userId, string friendId)
        {
            m_chatManager.UnFriend(userId, friendId);
        }

        /**
         * @overview: searchFriend
         */
        public void SearchFriend(string friendId)
        {
            m_chatManager.SearchFriend(friendId);
        }

        #endregion

        #region Get List
        /**
         * @overview: return the List that contains all the users of the system
         */
        public List<UserPerson> GetUserList()
        {
            return m_chatManager.GetUserList();
        }

        /**
         * @overview: return the List that contains friends of specific User
         */
        public List<UserPerson> GetFriendList(string userId)
        {
            return m_chatManager.GetFriendList(userId);
        }

        /**
         * @overview: return the List that contains all member of specific group
         */
        public List<UserPerson> GetMemberList(string groupId)
        {
            return m_chatManager.GetMemberList(groupId);
        }

        #endregion

        #region Message
        public void BroadcastOnlineStatus(string displayName, char status, string connId)
        {
            Clients.AllExcept(connId).BroadcastOnlineStatus(displayName, status);
        }

        public void SendMessageToServer(Messages m)
        {
            m_chatManager.SendMessageToServer(m);
        }

        public void SendMessageToClient(string connectionId, Messages m)
        {
            Clients.Client(connectionId).SendMessageToClient(m); 
        }

        public void SendOfflineMessage(string connectionId, List<Messages> list)
        {
            Clients.Client(connectionId).SendOfflineMessage(list);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcf_chat
{
    public class Room
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public bool IsPublic { get; set; }

        public int AuthorID { get; set; }

        public bool IsSingle { get; set; }

        public int Guest { get; set; }

        private List<UserInfo> users;

        public Room()
        {
            users = new List<UserInfo>();
        }

        public List<UserInfo> GetUsersInRoom()
        {
            return users;
        }

        public void AddUser(UserInfo user)
        {
            users.Add(user);
        }

        public void RemoveUser(UserInfo user)
        {
            users.Remove(user);
        }
    }
}

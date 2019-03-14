using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcf_chat
{
    class Room
    {
        public int ID { get; set; }

        private List<ServerUser> users;

        public List<ServerUser> GetUsersInRoom()
        {
            return users;
        }

        public void AddUser(ServerUser user)
        {
            users.Add(user);
        }

        public void RemoveUser(ServerUser user)
        {
            users.Remove(user);
        }
    }
}

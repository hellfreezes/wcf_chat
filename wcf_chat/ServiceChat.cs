using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace wcf_chat
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> users = new List<ServerUser>();
        int nextId = 1;

        public int Connect(string name)
        {
            ServerUser user = new ServerUser()
            {
                Id = nextId,
                Name = name,
                Operation = OperationContext.Current
            };

            nextId++;

            SendMsg(user.Name + " подключился к чату", 0);
            users.Add(user);

            return user.Id;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.Id == id);

            if (user != null)
            {
                users.Remove(user);
                SendMsg(user.Name + " покинул чат.", 0);
            }
        }

        public void SendMsg(string msg, int id)
        {
            foreach (ServerUser item in users)
            {
                string answer = DateTime.Now.ToShortTimeString() + ": ";

                var user = users.FirstOrDefault(i => i.Id == id);

                if (user != null)
                {
                    answer += user.Name + " ";
                }

                answer += msg;

                item.Operation.GetCallbackChannel<IServiceChatCallback>().MsgCallback(answer);
            }
        }
    }
}

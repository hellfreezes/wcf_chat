﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace wcf_chat
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IServiceChat" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IServiceChatCallback))]
    public interface IServiceChat
    {
        [OperationContract]
        int Connect(string name, string password);

        [OperationContract]
        void Disconnect(int id);

        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, int id);

        [OperationContract(IsOneWay = true)]
        void UpdateUsersList();

        [OperationContract(IsOneWay = true)]
        void RoomListRequest(int userId);
    }

    public interface IServiceChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void MsgCallback(string msg);

        [OperationContract(IsOneWay = true)]
        void UpdateUserListCallback(List<UserInfo> users);

        [OperationContract(IsOneWay = true)]
        void UpdateRoomListCallback(List<Room> rooms);
    }
}

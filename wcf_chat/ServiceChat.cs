using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace wcf_chat
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        Dictionary<int, ServerUser> users = new Dictionary<int, ServerUser>();

        List<Room> rooms = new List<Room>();

        private String dbFileName;
        private SQLiteConnection m_dbConn;
        private SQLiteCommand m_sqlCmd;

        const string DATA_BASE_PATH = "iComm.db";

        public int Connect(string name, string password)
        {
            ConnectToBase();

            ServerUser user = VerifyAccount(name, password);

            if (user == null)
            {
                // TODO: обработать!!!!!!
                // Пользователь не найден.
                return 0;
            }

            SendMsg(": " + user.Name + "(" + user.Id + ")" + ": подключился к чату", 0);

            users.Add(user.Id, user);
            

            return user.Id;
        }

        public void Disconnect(int id)
        {
            if (!users.ContainsKey(id))
            {
                Console.WriteLine("Пользователь с ID:{0} не найден", id);
                return;
            }
            SendMsg(users[id].Name + " покинул чат.", 0);
            users.Remove(id);
        }

        public void SendMsg(string msg, int id)
        {
            foreach (KeyValuePair<int, ServerUser> item in users)
            {
                string answer = DateTime.Now.ToShortTimeString() + ": ";

                var user = users[id];

                if (user != null) 
                {
                    answer += user.Name + " ";
                }

                answer += msg;

                item.Value.Operation.GetCallbackChannel<IServiceChatCallback>().MsgCallback(answer);
            }
        }

        public void UpdateUsersList()
        {
            foreach (KeyValuePair<int, ServerUser> item in users)
            {
                //TODO: Каждый раз генерирует список пользователей. ИСПРАВИТЬ!
                item.Value.Operation.GetCallbackChannel<IServiceChatCallback>().UpdateUserListCallback(GetUsersInfo(users));
            }
        }

        // Запрос на получение списка комнат доступных для конкретного юзера (userID)
        public void RoomListRequest(int userId)
        {
            List<Room> result = GetAllPublicRooms();
            users[userId].Operation.GetCallbackChannel<IServiceChatCallback>().UpdateRoomListCallback(result);
        }

        List<UserInfo> GetUsersInfo(Dictionary<int, ServerUser> users)
        {
            List<UserInfo> usersInfo = new List<UserInfo>();
            foreach (KeyValuePair<int, ServerUser> user in users)
            {
                usersInfo.Add(new UserInfo() { ID = user.Value.Id, Name = user.Value.Name });
            }
            return usersInfo;
        }

        private Room CreateRoom()
        {
            //TODO: Комната должна записываться в Базу данных и 
            //ID должен присваиваться из БАЗЫ
            Room r = new Room()
            {
                ID = 0
            };

            return r;
        }

        private void ConnectToBase()
        {
            m_dbConn = new SQLiteConnection();
            m_sqlCmd = new SQLiteCommand();

            dbFileName = DATA_BASE_PATH;

            try
            {
                m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                Console.WriteLine("Connected to SQL base");
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error while connecting to SQL base: " + ex.Message);
            }
        }

        private ServerUser VerifyAccount(string login, string password)
        {
            DataTable dTable = new DataTable();

            String sqlQuery;

            ServerUser user;

            if (m_dbConn.State != ConnectionState.Open)
            {
                Console.WriteLine("SQL base not connected");
                return null;
            }

            try
            {
                sqlQuery = "SELECT * FROM `users` WHERE `login`='" + login + "' AND `password`='" + password + "'";


                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, m_dbConn);
                adapter.Fill(dTable);

                if (dTable.Rows.Count > 0)
                {
                    Console.WriteLine("Пользователь " + login + " верификация пройдена.");

                    user = new ServerUser()
                    {
                        Id = int.Parse(dTable.Rows[0][0].ToString()),
                        Name = dTable.Rows[0][3].ToString(),
                        Operation = OperationContext.Current
                    };

                    return user;

                }
                else
                    Console.WriteLine("Пользователь " + login + " не найден.");
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null;
        }

        private List<Room> GetAllPublicRooms()
        {
            List<Room> list = new List<Room>();

            DataTable dTable = new DataTable();

            String sqlQuery;

            if (m_dbConn.State != ConnectionState.Open)
            {
                Console.WriteLine("SQL base not connected");
                return null;
            }

            try
            {
                sqlQuery = "SELECT * FROM `rooms` WHERE `public`='TRUE'";


                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, m_dbConn);
                adapter.Fill(dTable);

                if (dTable.Rows.Count > 0)
                {
                    foreach(DataRow row in dTable.Rows)
                    {

                        Room r = new Room()
                        {
                            ID = int.Parse(row[0].ToString()),
                            Name = row[1].ToString(),
                            IsPublic = bool.Parse( row[2].ToString()),
                            AuthorID = int.Parse(row[3].ToString())
                        };

                        
                        list.Add(r);
                    }

                    return list; //Вернуть список публичных комнат

                }
                else
                    return null; //Публичных комнат не найдено или произошла ошибка
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null; //Публичных комнат не найдено или произошла ошибка
        }
    }
}

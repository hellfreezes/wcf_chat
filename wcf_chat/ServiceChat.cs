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
        List<ServerUser> users = new List<ServerUser>();

        private String dbFileName;
        private SQLiteConnection m_dbConn;
        private SQLiteCommand m_sqlCmd;

        const string DATA_BASE_PATH = "D:/CSharpProjects/wcf_chat/iComm.db";

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

        public void UpdateUsersList()
        {
            foreach (ServerUser item in users)
            {
                item.Operation.GetCallbackChannel<IServiceChatCallback>().UpdateUserListCallback(null);
            }
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

        
    }
}

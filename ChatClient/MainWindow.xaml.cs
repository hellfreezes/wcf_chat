using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;
using ChatClient.ServiceChat;

namespace ChatClient
{
    public class CustomPropertys
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServiceChatCallback
    {
        bool isConnected = false;
        ServiceChatClient client;
        int ID;

        public string Login { get; set; }
        public string Password { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadPropertys();
        }

        void ConnectUser()
        {
            if (!isConnected)
            {
                client = new ServiceChatClient(new InstanceContext(this));
                ID = client.Connect(Login, Password);
                client.UpdateUsersList();
                client.RoomListRequest(ID);
                tbUserName.IsEnabled = false;
                btnConnect.Content = "Disconnect";
                isConnected = true;
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
                client.UpdateUsersList();
                //TODO: НИФИГА не работает выбор первого элемента. Исправить!
                lbRooms.SelectedIndex = 0;
                tbUserName.IsEnabled = true;
                btnConnect.Content = "Connect";
                client = null;
                isConnected = false;
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                
                DisconnectUser();
            } else
            {
                ConnectUser();
            }
        }

        public void MsgCallback(string msg)
        {
            lbChat.Items.Add(msg);
            //Прокрутка в конец списка
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
        }

        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void winMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client != null)
                {
                    client.SendMsg(tbMessage.Text, ID);
                    tbMessage.Text = string.Empty;
                }
            }
        }


        private void LoadPropertys()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("props.xml");
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                // обходим все дочерние узлы элемента user
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    // если узел - company
                    if (xnode.Name == "account")
                    {
                        Login = childnode.Value;
                        tbUserName.Text = Login;
                    }
                    // если узел age
                    if (xnode.Name == "password")
                    {
                        Password = childnode.Value;
                    }
                }
            }
        }

        private void SavePropertys(CustomPropertys props)
        {
            XDocument xDoc = new XDocument();

            // создаем новый элемент user
            XElement userElem = new XElement("user");

            // создаем элементы 
            XElement companyElem = new XElement("account", props.Login);
            XElement ageElem = new XElement("password", props.Password);

            userElem.Add(companyElem);
            userElem.Add(ageElem);

            xDoc.Add(userElem);

            xDoc.Save("props.xml");
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow winAccount = new AccountWindow(Login, Password);
            winAccount.Owner = this;
            winAccount.WindowClosed += OnAccountWindowClosed;
            winAccount.AccountSaved += OnAccountWindowsSaved;
            winAccount.Show();
            btnAccount.IsEnabled = false;
        }

        private void OnAccountWindowClosed(object sender, EventArgs e)
        {
            btnAccount.IsEnabled = true;
        }

        private void OnAccountWindowsSaved(object sender, AccountEventArgs e)
        {
            Login = e.Account;
            Password = e.Password;

            tbUserName.Text = Login;
            SavePropertys(new CustomPropertys { Login = Login, Password = Password });
        }

        public void UpdateUserListCallback(UserInfo[] users)
        {
            if (users == null)
                return;

            lbUsers.Items.Clear();
            
            foreach(UserInfo user in users)
            {
                lbUsers.Items.Add(user.Name);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void UpdateRoomListCallback(Room[] rooms)
        {
            lbRooms.Items.Clear();

            foreach(Room r in rooms)
            {
                lbRooms.Items.Add(r.Name);
            }
        }
    }
}

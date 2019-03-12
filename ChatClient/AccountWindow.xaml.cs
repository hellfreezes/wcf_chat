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
using System.Windows.Shapes;

namespace ChatClient
{
    public class AccountEventArgs : EventArgs
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }


    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class AccountWindow : Window
    {
        public event EventHandler WindowClosed;
        public event EventHandler<AccountEventArgs> AccountSaved;

        public AccountWindow(string login, string password)
        {
            InitializeComponent();
            tbLogin.Text = login;
            tbPassword.Text = password;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string account = tbLogin.Text.Trim();
            string password = tbPassword.Text.Trim();
            if (account != string.Empty && password != string.Empty)
            {
                AccountSaved?.Invoke(sender, new AccountEventArgs() { Account = account, Password = password });
                this.Close();
            }
            else
            {
                MessageBox.Show("Некорректно введен логин или пароль");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowClosed?.Invoke(sender, e);
        }
    }
}

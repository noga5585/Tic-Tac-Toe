using System;
using System.Text;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;

namespace Tic_Tac_Toe
{
    /// <summary>
    /// Interaction logic for ConnectToGame.xaml
    /// </summary>
    public partial class ConnectToGame : Window
    {
        public static string start;
        public static Socket socketToServer;
        public static bool canStart = false;
        public static string username = ""; 

        public ConnectToGame()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Check if the user details are good
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Check(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.logIn && !MainWindow.signUp)
            {
                MessageBox.Show("Choose whether you want to log in or sign up");
                return; 
            }
            if(password.Password == "" || name.Text == "")
            {
                MessageBox.Show("There must be at least one character in the username and password");
                return; 
            }
            string userCorrect = ConnectToServer();
            if(userCorrect  == "Yes")
            {
                canStart = true;
                MainWindow w = new MainWindow();
                w.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(start);
            }
        }
        /// <summary>
        /// Connected to the srever and get if the details in the database
        /// </summary>
        /// <returns></returns>
        private string ConnectToServer()
        {
            int port = 7000;
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            socketToServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketToServer.Connect(ipPoint);
            string message = String.Format("{0}-{1}-{2}",MainWindow.newUser, name.Text, password.Password);
            byte[] Senddata = Encoding.ASCII.GetBytes(message);
            socketToServer.Send(Senddata);
            byte[] data = new byte[1024];
            int bytes = 0;
            bytes = socketToServer.Receive(data, data.Length, 0);
            start = (Encoding.ASCII.GetString(data, 0, bytes));
            username = name.Text; 
            if(MainWindow.newUser)
            {
                string[] datafromserver = start.Split(',');
                if(datafromserver[0] == "Ok")
                {
                    socketToServer.Close();
                    return "Yes";
                }
            }
            else if (start == "Ok")
            {
                socketToServer.Close();
                return "Yes";
            }  
            socketToServer.Close();
            return start; 
            
        }
        /// <summary>
        /// Back to menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back(object sender, RoutedEventArgs e)
        {
            canStart = false;
            MainWindow.logIn = false;
            MainWindow.signUp = false;
            MainWindow.newUser = false; 
            MainWindow w = new MainWindow();
            w.Show();
            this.Close();
        }
    }
}

using System;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tic_Tac_Toe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool computerGame;
        public static string start;
        public static Socket socketToServer;
        public static bool signUp = false;
        public static bool logIn = false;
        public static bool newUser = false;
        public static string listOfPlayer; 

        public MainWindow()
        {
            InitializeComponent();
            Reset();
        }
        /// <summary>
        /// Reset the windows and the variables 
        /// </summary>
        public void Reset()
        {
            computerGame = false;
            Uri iconUri = new Uri("pack://application:,,,/img/BordMenu.jpg", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            if (signUp || logIn)
            {
                sign_up.IsEnabled = false;
                log_in.IsEnabled = false;
            }
        }
        /// <summary>
        /// if the user connected => connect to the server and start game with two player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Two_Player(object sender, RoutedEventArgs e)
        {
            if (ConnectToGame.canStart)
            {
                int port = 7000;
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                socketToServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketToServer.Connect(ipPoint);
                //send to srever two player 
                string message = String.Format("{0}-{1}", "two players", ConnectToGame.username);
                byte[] data = Encoding.ASCII.GetBytes(message);
                MainWindow.socketToServer.Send(data);
                byte[] data_from_serever = new byte[1024];
                int bytes = 0;
                bytes = socketToServer.Receive(data_from_serever, data_from_serever.Length, 0); // add message to the player to wait. 
                start = Encoding.ASCII.GetString(data_from_serever, 0, bytes);
                BoardGame w = new BoardGame();
                w.Show();
                this.Close();
            }
            else
                MessageBox.Show("You need to connect to your user before you can start");

        }
        /// <summary>
        /// start a game against the computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_With_Computer(object sender, RoutedEventArgs e)
        {
            if (ConnectToGame.canStart)
            {
                computerGame = true;
                BoardGame w = new BoardGame();
                w.Show();
                this.Close();
            }
            else
                MessageBox.Show("You need to connect to your user before you can start");

        }
        /// <summary>
        /// open a help window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Help_Button(object sender, RoutedEventArgs e)
        {
            SoundPlayer player = new SoundPlayer(@"sound/help.wav");
            player.Load();
            player.Play();
            HelpWindow w = new HelpWindow();
            w.Show();
            this.Close();
        }
        
        private void Start_Two_Player(object sender, MouseEventArgs e)
        {
            wait.Content = "Hello, click the button and please wait for another player!";
            BoardMenu.Source = new BitmapImage(new Uri(@"img\bigTwoPlayer.jpeg", UriKind.Relative));
        }

        private void Real_Board_Menu(object sender, MouseEventArgs e)
        {
            wait.Content = ""; 
            BoardMenu.Source = new BitmapImage(new Uri(@"img/BordMenu.jpg", UriKind.Relative));
        }

        private void Start_Agaimst_Computer(object sender, MouseEventArgs e)
        {
            wait.Content = "Think you can beat the computer? lets get start to play!";
            BoardMenu.Source = new BitmapImage(new Uri(@"img\bigComputer.jpeg", UriKind.Relative));
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SignUp(object sender, RoutedEventArgs e)
        {
            ConnectToGame w = new ConnectToGame();
            w.Show();
            this.Close();
            signUp = true;
            logIn = false;
            newUser = true;
        }

        private void LogIn(object sender, RoutedEventArgs e)
        {
            ConnectToGame w = new ConnectToGame();
            w.Show();
            this.Close();
            logIn = true;
            signUp = false;
            newUser = false;
        }
        /// <summary>
        /// Connect to the server and Displays the five users with the highest number of wins
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WantResults(object sender, RoutedEventArgs e)
        {
            int port = 7000;
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            socketToServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketToServer.Connect(ipPoint);
            //send to srever two player 
            string message = "Want results-";
            byte[] data = Encoding.ASCII.GetBytes(message);
            MainWindow.socketToServer.Send(data);
            byte[] data_from_serever = new byte[1024];
            int bytes = 0;
            bytes = socketToServer.Receive(data_from_serever, data_from_serever.Length, 0); // add message to the player to wait. 
            listOfPlayer = Encoding.ASCII.GetString(data_from_serever, 0, bytes);
            ShowWinners w = new ShowWinners();
            w.Show();
            this.Close();
        }
    }
}

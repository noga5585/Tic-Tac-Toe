using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;



namespace Tic_Tac_Toe
{
    public delegate void MyEventHandler();

    /// <summary>
    /// Interaction logic for BoardGame.xaml
    /// </summary>
    public partial class BoardGame : Window
    {
        #region Members
        public static ButtonMark[] buttonMarks;
        public static GameResult thisGameResult;
        public static bool isPlayerXTurn;
        public static bool winTheComputer = false;
        public static Button btn1 = new Button();
        public static Button btn2 = new Button();
        private bool youO = false;
        private bool TheGameEnded = false;
        private Thread t;

        #endregion

        #region Constructor

        public BoardGame()
        {
            this.Top = 100;
            this.Left = 450;
            Uri iconUri = new Uri("pack://application:,,,/img/BordMenu.jpg", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            InitializeComponent();
            if (MainWindow.computerGame)
            {
                btn1.Content = "Leval1";
                btn2.Content = "Leval2";
                GridLeval1.Children.Add(btn1);
                GridLeval2.Children.Add(btn2);
                btn1.IsEnabled = false;
                btn1.Click += How_Leval;
                btn2.Click += How_Leval;
            }
            else
            {
                t = new Thread(ThreadReceiveData);
                t.Start();
            }
            NewGame();
        }
        #endregion

        private void ThreadReceiveData()
        {
            TheGameEnded = false;
            while (true)
            {
                if (TheGameEnded)
                {
                    MainWindow.socketToServer.Close();
                    break;
                }
                ReceiveData();
            }
        }


        // Initializes the game to a new game
        private void NewGame()
        {
            Label[] labelArray = { Label0_0, Label0_1, Label0_2, Label1_0, Label1_1, Label1_2, Label2_0, Label2_1, Label2_2 };
            for (int i = 0; i < labelArray.Length; i++)
            {
                labelArray[i].Content = "";
            }
            // Create an array with nine cells with a default value of free.
            buttonMarks = new ButtonMark[9];
            for (int i = 0; i < buttonMarks.Length; i++)
            {
                buttonMarks[i] = ButtonMark.Free;
            }
            EndedGrid.Children.Clear(); // Deletes the end screen
            isPlayerXTurn = true;
            winTheComputer = false;
            SoundPlayer player = new SoundPlayer(@"sound/newGame.wav");
            player.Load();
            player.Play();
            youO = false;
            TheGameEnded = false;
            user.Content = "Hello" + ConnectToGame.username; 
            if (!MainWindow.computerGame)
            {
                if (MainWindow.start == "False")
                {
                    howIs.Content = "Player 2 turn - X";
                    //howIs.Background = Brushes.Red;
                    youO = true;
                }
                else
                {
                    howIs.Content = "My turn - X";
                    //howIs.Background = Brushes.Red;
                }
            }    
        }

        /// <summary>
        /// event - if the player want do a move.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_On_Button(object sender, RoutedEventArgs e)
        {
            Button[] buttonArray = { Button0_0, Button0_1, Button0_2, Button1_0, Button1_1, Button1_2, Button2_0, Button2_1, Button2_2 };
            Button btn = (Button)sender;  //Gets values ​​of a button pressed
            if (btn == null || MainWindow.start == "False")
            {
                return;
            }
            for (int index = 0; index < buttonArray.Length; index++)
            {
                if (buttonArray[index] == btn) // check the index of the pressed button
                {
                    if (buttonMarks[index] == ButtonMark.Free)
                    {
                        Mark_It(index, true);
                        if (!MainWindow.computerGame)
                        {
                            SendMove(index);
                            MainWindow.start = "False";
                            if (youO)
                            {
                                howIs.Content = "Player 2 turn - X";
                            }
                            else
                                howIs.Content = "Player 2 turn - O";
                        }
                        break;
                    }
                }
            }
            isPlayerXTurn = !isPlayerXTurn;
        }
        /// <summary>
        /// the playrt choose which level he want to play against the computer 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void How_Leval(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn == btn1)
            {
                btn1.IsEnabled = false;
                btn2.IsEnabled = true;
            }
            else
            {
                btn1.IsEnabled = true;
                btn2.IsEnabled = false;
            }
            NewGame();
        }
        /// <summary>
        /// get the other player move from the server
        /// </summary>
        private void ReceiveData()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            if (MainWindow.start == "False")
            {
                try
                {
                    bytes = MainWindow.socketToServer.Receive(data, data.Length, 0);
                    string data1 = Encoding.ASCII.GetString(data, 0, bytes);
                    if (data1 == "ok")
                    {
                        MainWindow.socketToServer.Close();
                        return;
                    }
                    string[] fromPlayer = data1.Split('-');
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (youO)
                        {
                            howIs.Content = "My turn - O";
                        }
                        else
                            howIs.Content = "My turn - X";
                        if (data1 == "You won automatically")
                        {
                            t.Abort();
                            TheGameEnded = true;
                            if (youO)
                            {
                                The_Game_Ended(ButtonMark.Cross);
                                return;
                            }
                            else
                            {
                                The_Game_Ended(ButtonMark.Noutht);
                                return;
                            }
                        }
                        MainWindow.start = fromPlayer[1];
                        bool ended = bool.Parse(fromPlayer[2]);
                        if (ended)
                        {
                            t.Abort();
                            Mark_It(int.Parse(fromPlayer[0]), false);
                            TheGameEnded = true;
                            return;
                        }
                        else
                        {
                            Mark_It(int.Parse(fromPlayer[0]), false);
                        }
                    }));
                }
                catch (InvalidCastException e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        /// <summary>
        /// send the move o the server
        /// </summary>
        /// <param name="move"></param>
        private void SendMove(int move)
        {
            string result = "notEnded";
            CheckForWinner.Chack_For_Winner(buttonMarks);
            if (thisGameResult == GameResult.Tie)
            {
                result = "Tie";
            }
            if (thisGameResult == GameResult.Xwin)
            {
                result = "X";
            }
            if (thisGameResult == GameResult.Owin)
            {
                result = "O";
            }
            string message = String.Format("{0}-{1}", move, result);
            byte[] data = Encoding.ASCII.GetBytes(message);
            MainWindow.socketToServer.Send(data);
        }

        
        private int Computer_Turn()
        {
            ButtonMark[] array = new ButtonMark[9];
            Array.Copy(buttonMarks, array, buttonMarks.Length);
            int cell = BestComputerMove.Select_A_Cell(array, isPlayerXTurn);
            buttonMarks[cell] = ButtonMark.Cross;
            return cell;
        }
        private void Mark_It(int index, bool myTurn)
        {
            Label[] labelArray = { Label0_0, Label0_1, Label0_2, Label1_0, Label1_1, Label1_2, Label2_0, Label2_1, Label2_2 };
            if (MainWindow.computerGame)
            {
                if (isPlayerXTurn)
                {
                    labelArray[index].Content = " X";
                    labelArray[index].Foreground = Brushes.Red;
                    buttonMarks[index] = ButtonMark.Noutht;
                }
            }
            CheckForWinner.Chack_For_Winner(buttonMarks);
            if (thisGameResult != GameResult.Nothing)
            {
                Check_The_Game_Result();
                return; 
            }
            // computer move
            if (MainWindow.computerGame)
            {
                isPlayerXTurn = !isPlayerXTurn;
                int indexO = Computer_Turn();
                labelArray[indexO].Content = " O";
                labelArray[indexO].Foreground = Brushes.Yellow;
                buttonMarks[indexO] = ButtonMark.Cross;
            }
            // O player move
            else if ((myTurn && !youO) || (!myTurn && youO))
            {
                buttonMarks[index] = ButtonMark.Noutht;
                labelArray[index].Content = " X";
                labelArray[index].Foreground = Brushes.Red;
            }
            else if ((!myTurn && !youO) || (myTurn && youO))
            {
                buttonMarks[index] = ButtonMark.Cross;
                labelArray[index].Content = " O";
                labelArray[index].Foreground = Brushes.Yellow;
            }
            CheckForWinner.Chack_For_Winner(buttonMarks);
            if (thisGameResult != GameResult.Nothing)
            {
                Check_The_Game_Result();
            }
        }

        /// <summary>
        /// check the game result. if the game is ended send to finish method. 
        /// </summary>
        private void Check_The_Game_Result()
        {
            if (CheckForWinner.Chack_For_Winner(buttonMarks) == GameResult.Xwin)
            {
                The_Game_Ended(ButtonMark.Noutht);
            }
            else if (CheckForWinner.Chack_For_Winner(buttonMarks) == GameResult.Owin)
            {
                The_Game_Ended(ButtonMark.Cross);
            }
            else if (CheckForWinner.Chack_For_Winner(buttonMarks) == GameResult.Tie)
                The_Game_Ended(ButtonMark.Free);
        }

        /// <summary>
        /// Plays a victory song and displays a closing screen by who won
        /// </summary>
        /// <param name="howWin"></param>
        private void The_Game_Ended(ButtonMark howWin)
        {
            if (howWin == ButtonMark.Cross && MainWindow.computerGame)
            {
                SoundPlayer player2 = new SoundPlayer(@"sound/computerWin.wav");
                player2.Load();
                player2.Play();
            }

            Image img = new Image();
            if (howWin == ButtonMark.Noutht)
            {
                scoreX.Content = int.Parse(scoreX.Content.ToString()) + 1;
                img.Source = new BitmapImage(new Uri(@"img\Xwin.png", UriKind.Relative));
            }
            else if (howWin == ButtonMark.Cross)
            {
                scoreO.Content = int.Parse(scoreO.Content.ToString()) + 1;
                img.Source = new BitmapImage(new Uri(@"img\Owin.png", UriKind.Relative));
            }
            else
            {
                img.Source = new BitmapImage(new Uri(@"img\Tie.png", UriKind.Relative));
            }
            EndedGrid.Children.Add(img);
            if (!MainWindow.computerGame || howWin != ButtonMark.Cross)
            {
                SoundPlayer player = new SoundPlayer(@"sound/theWinner.wav");
                player.Load();
                player.Play();
            }
            TheGameEnded = true;
            if (!MainWindow.computerGame)
                t.Abort();

        }
        // Returns to the menu and Resets the scores
        private void Back_Menu(object sender, RoutedEventArgs e)
        {
            if (!TheGameEnded && !MainWindow.computerGame)
            {
                string message = String.Format("exit");
                byte[] data = Encoding.ASCII.GetBytes(message);
                MainWindow.socketToServer.Send(data);
            }
            TheGameEnded = true;
            scoreX.Content = 0;
            scoreO.Content = 0;
            MainWindow w = new MainWindow();
            w.Show();
            this.Close();
        }

        private void My_Game(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.LeftShift)
            {
                winTheComputer = true;
            }
        }
        
        private void Start_Over(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.computerGame)
            {
                Back_Menu(sender, e);
            }
            else
                NewGame(); 
        }
    }
}


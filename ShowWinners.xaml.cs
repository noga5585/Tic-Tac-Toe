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

namespace Tic_Tac_Toe
{
    /// <summary>
    /// Interaction logic for ShowWinners.xaml
    /// </summary>
    public partial class ShowWinners : Window
    {
        public ShowWinners()
        {
            InitializeComponent();
            ShowResults();

        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow w = new MainWindow();
            w.Show();
            this.Close();
        }
        private void ShowResults()
        {
            string[] newlistOfPlayers = MainWindow.listOfPlayer.Split(')');
            string thePlayerDetails;
            string[] splitDetails;
            for (int i = 0; i < MainWindow.listOfPlayer.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        thePlayerDetails = newlistOfPlayers[i];
                        splitDetails = thePlayerDetails.Split(',');
                        player1name.Content = splitDetails[1].Replace("\\n", "").Replace("\\r", ""); 
                        player1score.Content = splitDetails[4];
                        continue;
                    case 1:
                        thePlayerDetails = newlistOfPlayers[i];
                        splitDetails = thePlayerDetails.Split(',');
                        player2name.Content = splitDetails[2].Replace("\\n", "").Replace("\\r", ""); 
                        player2score.Content = splitDetails[5];
                        continue;
                    case 2:
                        thePlayerDetails = newlistOfPlayers[i];
                        splitDetails = thePlayerDetails.Split(',');
                        player3name.Content = splitDetails[2].Replace("\\n", "").Replace("\\r", ""); 
                        player3score.Content = splitDetails[5];
                        continue;
                    case 3:
                        thePlayerDetails = newlistOfPlayers[i];
                        splitDetails = thePlayerDetails.Split(',');
                        player4name.Content = splitDetails[2].Replace("\\n", "").Replace("\\r", ""); 
                        player4score.Content = splitDetails[5];
                        continue;
                    case 4:
                        thePlayerDetails = newlistOfPlayers[i];
                        splitDetails = thePlayerDetails.Split(',');
                        player5name.Content = splitDetails[2].Replace("\\n", "").Replace("\\r", ""); 
                        player5score.Content = splitDetails[5];
                        continue;
                    default:
                        break;
                }

            }
        }
    }
}

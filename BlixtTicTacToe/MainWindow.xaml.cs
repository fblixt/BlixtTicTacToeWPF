using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlixtTicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool xIsUp = true;
        private GameLogic gameLogic = new();
        private bool gameOver = false;

        private Image GetButtonImage(string fileName)
        {
            Image image = new Image
            {
                Source = new BitmapImage(new Uri($"/Images/{fileName}.png", UriKind.Relative)),
                Stretch = Stretch.Uniform
            };

            return image;
        }

        private void NewGame(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = new();

            if (gameOver)
            {
                SetNewGame();
            }
            else
            {
                result = MessageBox.Show("Are you sure you want to start a new game?", "New Game", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SetNewGame();
                }
                else if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
        }
        private void SetNewGame()
        {
            foreach (var child in Row1.Children)
            {
                if (child is Button button && button != null)
                {
                    button.Content = null;
                    button.IsEnabled = true;
                }
            }
            foreach (var child in Row2.Children)
            {
                if (child is Button button && button != null)
                {
                    button.Content = null;
                    button.IsEnabled = true;
                }
            }
            foreach (var child in Row3.Children)
            {
                if (child is Button button && button != null)
                {
                    button.Content = null;
                    button.IsEnabled = true;
                }
            }
            PlayerX.Background = Brushes.LightGreen;
            PlayerO.Background = Brushes.LightGray;
            xIsUp = true;
            gameOver = false;
            gameLogic.NewGame();
        }
        private void GameOver()
        {
            foreach (var child in Row1.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = false;
                }
            }
            foreach (var child in Row2.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = false;
                }
            }
            foreach (var child in Row3.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = false;
                }
            }
        }

        private void SetMark(object sender, RoutedEventArgs e)
        {
            if (gameOver) 
            {
                MessageBox.Show($"The game is over!\nStart a new game!");
                return;
            }

            if (sender is Button clickedButton && clickedButton.Content == null)
            {
                int row = int.Parse(clickedButton.Tag.ToString().Split(',')[0]);
                int col = int.Parse(clickedButton.Tag.ToString().Split(',')[1]);

                if (gameLogic.SetMark(row, col))
                {
                    clickedButton.Content = GetButtonImage(gameLogic.CurrentPlayer);
                    
                    string winner = gameLogic.CheckWinner();
                    if (winner != null)
                    {
                        gameOver = true;
                        var resultWindow = new ResultWindow(winner);
                        GameOver();
                        resultWindow.ShowDialog();
                    }
                    else if (gameLogic.IsDraw())
                    {
                        gameOver = true;
                        GameOver();
                        PlayerX.Background = Brushes.LightGray;
                        PlayerO.Background = Brushes.LightGray;
                        MessageBox.Show("It's a draw!");
                    }
                    else
                    ChangePlayer();
                }
            }
        }

        private void ChangePlayer()
        {
            xIsUp = !xIsUp;
            if (xIsUp) 
            {
                PlayerX.Background = Brushes.LightGreen;
                PlayerO.Background = Brushes.LightGray;
            }
            else
            {
                PlayerX.Background = Brushes.LightGray;
                PlayerO.Background = Brushes.LightGreen;
            }
        }
    }
}
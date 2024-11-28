using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

            TopLeftButton.Tag = (row: 0, col: 0);
            TopCenterButton.Tag = (row: 0, col: 1);
            TopRightButton.Tag = (row: 0, col: 2);

            CenterLeftButton.Tag = (row: 1, col: 0);
            CenterCenterButton.Tag = (row: 1, col: 1);
            CenterRightButton.Tag = (row: 1, col: 2);

            BottomLeftButton.Tag = (row: 2, col: 0);
            BottomCenterButton.Tag = (row: 2, col: 1);
            BottomRightButton.Tag = (row: 2, col: 2);
        }

        private GameLogic gameLogic = new();
        private bool xIsUp = true;
        private bool gameOver = false;
        private bool gameActive = false;
        private bool gameOnCom = true;
        private bool easyGame = true;
        private bool infiniteGame = false;
        //private int moves;
        private string CurrentPlayer => xIsUp ? "x" : "o";

        private Image GetGameButtonImage(string fileName)
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

            if (gameOver || !gameActive)
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
            EnableBoard();
            ClearBoard();

            PlayerXButton.Background = Brushes.LightGreen;
            PlayerOButton.ClearValue(Button.BackgroundProperty);

            //moves = 0;
            xIsUp = true;
            gameOver = false;
            gameActive = false;
            StartGameButton.IsEnabled = true;
            StartGameButton.Content = "Start Game";
            NewGameButton.ClearValue(Button.BackgroundProperty);
            EnableConfigButtons();
            gameLogic.NewGame();
            if ((string)PlayerXButton.Content == "Com - X")
            {
                ComAwaitStartSetUp();
                SelectX.Visibility = Visibility.Visible;
                SelectO.Visibility = Visibility.Visible;
            }
        }

        private void GameOver()
        {
            DisableBoard();
            StartGameButton.Content = "Start Game";
            StartGameButton.IsEnabled = false;
            NewGameButton.Background = Brushes.PowderBlue;
        }

        private void SetMark(object sender, RoutedEventArgs e)
        {
            if (gameOver)
            {
                MessageBox.Show($"The game is over!\nStart a new game!");
                return;
            }

            if (sender is Button clickedButton && clickedButton.Content == null && clickedButton.Tag is (int row, int col))
            {
                if (gameLogic.SetMark(row, col))
                {
                    clickedButton.Content = GetGameButtonImage(gameLogic.CurrentPlayer);

                    if (!gameActive)
                    {
                        ActivateGame();
                    }

                    string? winner = gameLogic.CheckWinner();
                    CheckGame(winner);

                    if (gameOnCom && !gameOver)
                    {
                        GetComMark();
                    }
                }
            }

            //if (infiniteGame && moves >= 6)
            //{
            //    DisableAllExceptCurrentPlayer(CurrentPlayer);

            //}
        }

        private void DisableAllExceptCurrentPlayer(string currentPlayer)
        {
            foreach (var child in Row1.Children)
            {
                if (child is Button button)
                {
                    if (button.Tag is (int row, int col) && gameLogic.GetPlayerAtPosition(row, col) != currentPlayer)
                    {
                        button.IsEnabled = false;
                    }
                }
            }
            foreach (var child in Row2.Children)
            {
                if (child is Button button)
                {
                    if (button.Tag is (int row, int col) && gameLogic.GetPlayerAtPosition(row, col) != currentPlayer)
                    {
                        button.IsEnabled = false;
                    }
                }
            }
            foreach (var child in Row3.Children)
            {
                if (child is Button button)
                {
                    if (button.Tag is (int row, int col) && gameLogic.GetPlayerAtPosition(row, col) != currentPlayer)
                    {
                        button.IsEnabled = false;
                    }
                }
            }
        }

        private void StartLoading()
        {
            var storyboard = (Storyboard)FindResource("SpinAnimation");
            storyboard.Begin();
        }

        private void StopLoading()
        {
            var storyboard = (Storyboard)FindResource("SpinAnimation");
            storyboard.Stop();
        }

        private async void GetComMark()
        {
            DisableBoard();
            LoadingCanvas.Visibility = Visibility.Visible;
            StartLoading();

            await Task.Delay(500);

            StopLoading();
            LoadingCanvas.Visibility = Visibility.Collapsed;
            EnableBoard();

            //string com = xIsUp ? "x" : "o";
            int row, col;
            if (easyGame)
                (row, col) = gameLogic.GetEasyComMark();
            else
                (row, col) = gameLogic.GetImpossibleComMark();

            Button comButton = row switch
            {
                0 when col == 0 => TopLeftButton,
                0 when col == 1 => TopCenterButton,
                0 when col == 2 => TopRightButton,
                1 when col == 0 => CenterLeftButton,
                1 when col == 1 => CenterCenterButton,
                1 when col == 2 => CenterRightButton,
                2 when col == 0 => BottomLeftButton,
                2 when col == 1 => BottomCenterButton,
                2 when col == 2 => BottomRightButton,
                _ => throw new InvalidOperationException("Invalid position")
            };

            if (gameLogic.SetMark(row, col))
            {
                comButton.Content = GetGameButtonImage(gameLogic.CurrentPlayer);
                var winner = gameLogic.CheckWinner();
                CheckGame(winner);
            }

        }

        private void CheckGame(string? winner)
        {
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
                PlayerXButton.ClearValue(Button.BackgroundProperty);
                PlayerOButton.ClearValue(Button.BackgroundProperty);
                MessageBox.Show("It's a draw!");
            }
            else
                ChangePlayer();
        }

        private void ChangePlayer()
        {
            xIsUp = !xIsUp;
            if (xIsUp)
            {
                PlayerXButton.Background = Brushes.LightGreen;
                PlayerOButton.ClearValue(Button.BackgroundProperty);
            }
            else
            {
                PlayerXButton.ClearValue(Button.BackgroundProperty);
                PlayerOButton.Background = Brushes.LightGreen;
            }
            //moves++;
        }

        private void DisableConfigButtons()
        {
            if (OneOnOneButton.Background is SolidColorBrush OneOnOne && OneOnOne.Color != Colors.LightGreen)
            {
                OneOnOneButton.IsEnabled = false;
            }
            if (OneOnComButton.Background is SolidColorBrush OneOnCom && OneOnCom.Color != Colors.LightGreen)
            {
                OneOnComButton.IsEnabled = false;
            }
            if (EasyButton.Background is SolidColorBrush Easy && Easy.Color != Colors.LightGreen)
            {
                EasyButton.IsEnabled = false;
            }
            if (ImpossibleButton.Background is SolidColorBrush Impossible && Impossible.Color != Colors.LightGreen)
            {
                ImpossibleButton.IsEnabled = false;
            }
            if (StandardGameButton.Background is SolidColorBrush Standard && Standard.Color != Colors.LightGreen)
            {
                StandardGameButton.IsEnabled = false;
            }
            if (InfiniteGameButton.Background is SolidColorBrush Infinite && Infinite.Color != Colors.LightGreen)
            {
                InfiniteGameButton.IsEnabled = false;
            }
        }

        private void EnableConfigButtons()
        {
            OneOnOneButton.IsEnabled = true;
            OneOnComButton.IsEnabled = true;
            EasyButton.IsEnabled = true;
            ImpossibleButton.IsEnabled = true;
            StandardGameButton.IsEnabled = true;
            InfiniteGameButton.IsEnabled = true;
        }

        private void EnableBoard()
        {
            foreach (var child in Row1.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = true;
                }
            }
            foreach (var child in Row2.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = true;
                }
            }
            foreach (var child in Row3.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = true;
                }
            }
        }

        private void DisableBoard()
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

        private void ClearBoard()
        {
            foreach (var child in Row1.Children)
            {
                if (child is Button button && button != null)
                {
                    button.Content = null;
                }
            }
            foreach (var child in Row2.Children)
            {
                if (child is Button button && button != null)
                {
                    button.Content = null;
                }
            }
            foreach (var child in Row3.Children)
            {
                if (child is Button button && button != null)
                {
                    button.Content = null;
                }
            }
        }

        private void ActivateGame()
        {
            gameActive = true;
            StartGameButton.Content = "Game Started";
            DisableConfigButtons();
            StartGameButton.ClearValue(Button.FontWeightProperty);
            StartGameButton.ClearValue(Button.BorderThicknessProperty);
            StartGameButton.Height = 30;
            StartGameButton.Width = 100;
            SelectX.Visibility = Visibility.Hidden;
            SelectO.Visibility = Visibility.Hidden;
        }

        private void OneOnOne(object sender, RoutedEventArgs e)
        {
            gameOnCom = false;
            OneOnOneButton.Background = Brushes.LightGreen;
            OneOnComButton.ClearValue(Button.BackgroundProperty);
            EasyButton.Visibility = Visibility.Hidden;
            ImpossibleButton.Visibility = Visibility.Hidden;
            PlayerXButton.Content = "Player X";
            PlayerOButton.Content = "Player O";
            SelectX.Visibility = Visibility.Hidden;
            SelectO.Visibility = Visibility.Hidden;
            EnableBoard();
        }

        private void OneOnCom(object sender, RoutedEventArgs e)
        {
            gameOnCom = true;
            OneOnComButton.Background = Brushes.LightGreen;
            OneOnOneButton.ClearValue(Button.BackgroundProperty);
            EasyButton.Visibility = Visibility.Visible;
            ImpossibleButton.Visibility = Visibility.Visible;
            PlayerXButton.Content = "Player - X";
            PlayerOButton.Content = "Com - O";
            SelectX.Visibility = Visibility.Visible;
            SelectO.Visibility = Visibility.Visible;
            if ((string)PlayerXButton.Content == "Com - X")
            {
                ComAwaitStartSetUp();
            }
        }

        private void Easy(object sender, RoutedEventArgs e)
        {
            EasyButton.Background = Brushes.LightGreen;
            ImpossibleButton.ClearValue(Button.BackgroundProperty);
            easyGame = true;
        }

        private void Impossible(object sender, RoutedEventArgs e)
        {
            ImpossibleButton.Background = Brushes.LightGreen;
            EasyButton.ClearValue(Button.BackgroundProperty);
            easyGame = false;
        }

        private void StandardGame(object sender, RoutedEventArgs e)
        {
            StandardGameButton.Background = Brushes.LightGreen;
            InfiniteGameButton.ClearValue(Button.BackgroundProperty);
            infiniteGame = false;
        }

        private void InfiniteGame(object sender, RoutedEventArgs e)
        {
            InfiniteGameButton.Background = Brushes.LightGreen;
            StandardGameButton.ClearValue(Button.BackgroundProperty);
            infiniteGame = true;
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            ActivateGame();
            if (gameOnCom && (string)PlayerXButton.Content == "Com - X")
                GetComMark();
        }

        private void ComAwaitStartSetUp()
        {
            StartGameButton.FontWeight = FontWeights.Bold;
            StartGameButton.BorderThickness = new Thickness(2);
            StartGameButton.Height = 35;
            StartGameButton.Width = 110;
            DisableBoard();
        }

        private void PlayerX(object sender, RoutedEventArgs e)
        {
            if (!gameActive && gameOnCom)
            {
                PlayerXButton.Content = "Player - X";
                PlayerOButton.Content = "Com - O";
                StartGameButton.ClearValue(Button.FontWeightProperty);
                StartGameButton.ClearValue(Button.BorderThicknessProperty);
                StartGameButton.Height = 30;
                StartGameButton.Width = 100;
                EnableBoard();
            }
        }

        private void PlayerO(object sender, RoutedEventArgs e)
        {
            if (!gameActive && gameOnCom)
            {
                PlayerXButton.Content = "Com - X";
                PlayerOButton.Content = "Player - O";
                ComAwaitStartSetUp();
            }
        }
    }
}
using System.Reflection;
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

            TopLeftButton.Tag = (row: 0, col: 0);
            TopCenterButton.Tag = (row: 0, col: 1);
            TopRightButton.Tag = (row: 0, col: 2);

            CenterLeftButton.Tag = (row: 1, col: 0);
            CenterCenterButton.Tag = (row: 1, col: 1);
            CenterRightButton.Tag = (row: 1, col: 2);

            BottomLeftButton.Tag = (row: 2, col: 0);
            BottomCenterButton.Tag = (row: 2, col: 1);
            BottomRightButton.Tag = (row: 2, col: 2);

            EasyButton.Visibility = Visibility.Hidden;
            ImpossibleButton.Visibility = Visibility.Hidden;
        }

        private bool xIsUp = true;
        private GameLogic gameLogic = new();
        private bool gameOver = false;
        private bool gameActive = false;
        private bool gameOnCom = false;

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
            PlayerO.ClearValue(Button.BackgroundProperty);

            xIsUp = true;
            gameOver = false;
            gameActive = false;
            ActivateConfigButtons();
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

        private async void SetMark(object sender, RoutedEventArgs e)
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

                    if (!gameActive) { LockConfigButtons(); }

                    string? winner = gameLogic.CheckWinner();
                    CheckGame(winner);

                    if (gameOnCom && !gameOver)
                    {
                        await Task.Delay(1000);

                        var position = gameLogic.GetComMark();
                        (row, col) = position.Value;
                        Button comButton = GetComButton(row, col);

                        if (gameLogic.SetMark(row, col))
                        {
                            comButton.Content = GetGameButtonImage(gameLogic.CurrentPlayer);
                            winner = gameLogic.CheckWinner();
                            CheckGame(winner);
                        }
                    }
                }
            }
        }

        private Button GetComButton(int row, int col)
        {
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
            return comButton;
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
                PlayerX.ClearValue(Button.BackgroundProperty);
                PlayerO.ClearValue(Button.BackgroundProperty);
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
                PlayerX.Background = Brushes.LightGreen;
                PlayerO.ClearValue(Button.BackgroundProperty);
            }
            else
            {
                PlayerX.ClearValue(Button.BackgroundProperty);
                PlayerO.Background = Brushes.LightGreen;
            }
        }

        private void LockConfigButtons()
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
            gameActive = true;
        }
        private void ActivateConfigButtons()
        {
            OneOnOneButton.IsEnabled = true;
            OneOnComButton.IsEnabled = true;
            EasyButton.IsEnabled = true;
            ImpossibleButton.IsEnabled = true;
            StandardGameButton.IsEnabled = true;
            InfiniteGameButton.IsEnabled = true;
        }

        private void OneOnOne(object sender, RoutedEventArgs e)
        {
            gameOnCom = false;
            OneOnOneButton.Background = Brushes.LightGreen;
            OneOnComButton.ClearValue(Button.BackgroundProperty);
            EasyButton.Visibility = Visibility.Hidden;
            ImpossibleButton.Visibility = Visibility.Hidden;
        }

        private void OneOnCom(object sender, RoutedEventArgs e)
        {
            gameOnCom = true;
            OneOnComButton.Background = Brushes.LightGreen;
            OneOnOneButton.ClearValue(Button.BackgroundProperty);
            EasyButton.Visibility = Visibility.Visible;
            ImpossibleButton.Visibility = Visibility.Visible;
        }

        private void Easy(object sender, RoutedEventArgs e)
        {
            EasyButton.Background = Brushes.LightGreen;
            ImpossibleButton.ClearValue(Button.BackgroundProperty);
        }

        private void Impossible(object sender, RoutedEventArgs e)
        {
            ImpossibleButton.Background = Brushes.LightGreen;
            EasyButton.ClearValue(Button.BackgroundProperty);
        }

        private void StandardGame(object sender, RoutedEventArgs e)
        {
            StandardGameButton.Background = Brushes.LightGreen;
            InfiniteGameButton.ClearValue(Button.BackgroundProperty);
        }

        private void InfiniteGame(object sender, RoutedEventArgs e)
        {
            InfiniteGameButton.Background = Brushes.LightGreen;
            StandardGameButton.ClearValue(Button.BackgroundProperty);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace BlixtTicTacToe
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow(string winner)
        {
            InitializeComponent();

            ResultText.Text = "is the winner!";

            string imagePath = $"pack://application:,,,/Images/{winner.ToLower()}.png";
            ResultImage.Source = new BitmapImage(new Uri(imagePath));
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

using FinalProject;
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

namespace GolfScoreManager
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();

            BitmapImage theBitmap = new BitmapImage();
            theBitmap.BeginInit();
            String path = System.IO.Path.Combine(Environment.CurrentDirectory, @"assets\gb.jpeg");
            theBitmap.UriSource = new Uri(path, UriKind.Absolute);
            theBitmap.DecodePixelWidth = 500;
            theBitmap.EndInit();

            ImageBrush background = new ImageBrush(theBitmap);
            mainGrid.Background = background;

        }
    }

}

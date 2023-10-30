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
using AppCountriesExplorerWpf.Views;

namespace AppCountriesExplorerWpf
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
        /// <summary>
        /// Open the UserCountrolCountry into the Center Grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCountries_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new UserControlCountry();
        }
        /// <summary>
        /// Open the UserCountrolMap into the Center Grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Maps_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new UserControlMap();
        }
        /// <summary>
        /// Close the Application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

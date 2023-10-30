using AppCountriesExplorerWpf.Model.Services;
using AppCountriesExplorerWpf.Model;
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

namespace AppCountriesExplorerWpf.Views
{
    /// <summary>
    /// Interaction logic for UserControlMap.xaml
    /// </summary>
    public partial class UserControlMap : UserControl
    {
        #region Atributos


        private ApiService apiService;
        private NetworkService networkService;
        private DataService dataService;
        private List<Country> countries;
        bool load;


        #endregion
        public UserControlMap()
        {
            InitializeComponent();
            apiService = new ApiService();
            networkService = new NetworkService();
            dataService = new DataService();
            countries = new List<Country>();
            LoadCountriesAsync();
        }
        /// <summary>
        /// Método é chamado de forma assíncrona para carregar os países da Api ou da base de dados local.
        /// </summary>
        private async void LoadCountriesAsync()
        {

            LabelResult.Content = "Loading Countries...";
            var connection = networkService.CheckConnection();
            if (!connection.IsSucess)
            {

                load = false;
                await LoadLocalCountries();
                FillComBox();


            }
            else
            {
                load = true;
                await LoadApiCountries();
                FillComBox();



            }
            if (countries.Count == 0)
            {
                LabelResult.Content = " There's not internet connection" + Environment.NewLine +
                   "The countries didn't load" + Environment.NewLine + "Please try later!!!";

                LabelStatus.Content = "First startup must have internet connection!!!";
                return;
            }

            ComboBoxNameCountries.ItemsSource = countries;
           

            if (load)
            {
                LabelStatus.Content = string.Format("Countries loaded by Internet on {0:F}", DateTime.Now);
            }
            else
            {
                LabelStatus.Content = string.Format("Countries loaded by Data base on {0:F}", DateTime.Now);
            }
        }
        /// <summary>
        /// Método para preencher a Combobox
        /// </summary>
        private void FillComBox()
        {
            ComboBoxNameCountries.ItemsSource = countries;

        }
        private async Task LoadLocalCountries()
        {
            string pastaImagens = "C:\\Users\\daia_\\Desktop\\TRABALHOS DAIANE - CINEL\\Rafael\\AppCountriesExplorerWpf\\AppCountriesExplorerWpf\\Flags\\"; // Caminho para a pasta com as imagens  

            foreach (Country country in countries)
            {

                await Task.Run(() => country.ImagePath = pastaImagens + country.name.common + ".png");

            }
            ComboBoxNameCountries.ItemsSource = countries;
            countries = dataService.GetData();

        }
        /// Método preenche com a lista de countries a combobox.
        private async Task LoadApiCountries()
        {
            var response = await apiService.GetCountries("https://restcountries.com", "/v3.1/all");
            countries = (List<Country>)response.Result;

        }
        /// <summary>
        /// Representa o manipulador de evento SelectionChanged do ComboBoxNameCountries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ComboBoxNameCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var country = (Country)ComboBoxNameCountries.SelectedItem;
            if (load)
            {
                string openStreetMapUrl = country.maps.openStreetMaps;
                if (!openStreetMapUrl.StartsWith("http://") && !openStreetMapUrl.StartsWith("https://"))
                {
                    openStreetMapUrl = "http://" + openStreetMapUrl;
                }
                WebView.NavigationCompleted += WebView_NavegationCompleted;
                await WebView.EnsureCoreWebView2Async();
                WebView.CoreWebView2.Navigate(openStreetMapUrl);
            }

        }
        /// <summary>
        /// Manipulador de evento que se executa quando se completa a navegação em um control WebView. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WebView_NavegationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            const string javascript = @"
                                      var mapElement = document.getElementById('map');
                                      if(mapElement)
                                      { 
                                      var elementsToHide = document.querySelectorAll('h1, header, .primary, .secundary, #sidebar');
                                      for(var i = 0; i < elementsToHide.lenght; i++)
                                      {
                                        elementsToHide[i].style.display = 'none';
                                      }
                                        mapElement.style.height = '400 px'
                                      }
                                       var bodyElement = document.querySelector('body');
                                      if(bodyElement)
                                      {
                                        bodyElement.style.marginTop = '0';
                                        bodyElement.style.paddingTop = '0';
                                      }
                                       var contentElement = document.getElementById('content');
                                      if(contentElement)
                                      {
                                        contentElement.style.marginTop = '55px';
                                      }
                                      ";

            await WebView.ExecuteScriptAsync(javascript);

        }
    }
}

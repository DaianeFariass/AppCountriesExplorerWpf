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
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.ComponentModel;

namespace AppCountriesExplorerWpf.Views
{
    /// <summary>
    /// Interaction logic for UserControlCountry.xaml
    /// </summary>
    public partial class UserControlCountry : UserControl
    {
        #region Atributos
        private ApiService apiService;
        private NetworkService networkService;
        private DataService dataService;
        private List<Country> countries;
        private Progress<int> progress;
        private Progress<string> progre;
        #endregion
        public UserControlCountry()
        {
            InitializeComponent();
            apiService = new ApiService();
            networkService = new NetworkService();
            dataService = new DataService();
            countries = new List<Country>();

            //As instâncias do Progress são usadas para notificar e atualizar elementos da interface
            //do usuário (a barra de progresso e a imagem) à medida que novos valores e caminhos de
            //arquivo são recebidos.
            progress = new Progress<int>(value =>
            {
                ProgressBarStatus.Value = value;

            });
            progre = new Progress<string>(path =>
            {

                // Carregar a imagem a partir do caminho fornecido
                ImageFlagsLoading.Source = new BitmapImage(new Uri(path));
            });
            LoadCountriesAsync();
            AssignEventComboBox();
        }
        /// <summary>
        /// Método é chamado de forma assíncrona para carregar os países da Api ou da base de dados local.
        /// </summary>
        private async void LoadCountriesAsync()
        {
            bool load;
            LabelResult.Content = "Loading Countries...";
            var connection = networkService.CheckConnection();
            if (!connection.IsSucess)
            {
                await LoadLocalCountries();
                load = false;
                await ProgressFlags();
                //DownloadFlags();
                await FillComBoxAsync();
                AssignEventComboBox();
            }
            else
            {
                await LoadApiCountries();
                load = true;
                await ProgressFlags();
                await FillComBoxAsync();
                AssignEventComboBox();
            }
            if (countries.Count == 0)
            {
                LabelResult.Content = " There's not internet connection" + Environment.NewLine +
                   "The countries didn't load" + Environment.NewLine + "Please try later!!!";

                LabelStatus.Content = "First startup must have internet connection!!!";
                return;
            }

            LabelResult.Content = "Countries loaded...";

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
        /// Método instância um IProgress<int> para que a ProgressBar seja preenchida simultâneamente com a lista de countries na combobox.
        /// </summary>
        /// <returns><list type="Country"</returns>
        private async Task<List<Country>> FillComBoxAsync()
        {
            IProgress<int> progressReporter = progress;

            string pastaImagens = "C:\\Users\\daia_\\Desktop\\TRABALHOS DAIANE - CINEL\\Rafael\\AppCountriesExplorerWpf\\AppCountriesExplorerWpf\\Flags\\"; // Caminho para a pasta com as imagens  

            foreach (Country country in countries)
            {

                country.ImagePath = pastaImagens + country.name.common + ".png";

            }
            await Task.Run(() =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    // Relatar o progresso usando o objeto de progresso
                    progressReporter.Report(i);


                    // Simular um trabalho em segundo plano
                    Thread.Sleep(100);
                }

            });
            ComboBoxNameCountries.ItemsSource = countries;
            return countries;



        }
        /// <summary>
        /// Método assíncrono  carrega os países a partir da base de dados local.
        /// </summary>
        /// <returns>List<Country></returns>
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
        /// <summary>
        /// Método faz uma chamada assíncrona para uma API para obter a lista de países.
        /// </summary>
        /// <returns>List<Country></returns>
        private async Task LoadApiCountries()
        {
            var response = await apiService.GetCountries("https://restcountries.com", "/v3.1/all");
            countries = (List<Country>)response.Result;
            dataService.DeleteData();
            dataService.SaveData(countries);
        }
        /// <summary>
        /// Método que itera pelos países e relata o progresso do carregamento das imagens das bandeiras.
        /// </summary>
        /// <returns>Flags</returns>
        private async Task ProgressFlags()
        {
            IProgress<string> reporte = progre;
            string pastaImagens = "C:\\Users\\daia_\\Desktop\\TRABALHOS DAIANE - CINEL\\Rafael\\AppCountriesExplorerWpf\\AppCountriesExplorerWpf\\Flags\\"; // Caminho para a pasta com as imagens  
            foreach (Country country in countries)
            {

                country.ImagePath = pastaImagens + country.name.common + ".png";

                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                    reporte.Report(country.ImagePath);
                });


            }

        }
        /// <summary>
        /// Método que manipula a exibição da imagem da bandeira selecionada com base na seleção feita em uma combobox.
        /// </summary>
        private void ShowFlag()
        {
            byte[] imageData;
            var country = (Country)ComboBoxNameCountries.SelectedItem;
            string img = country.flags.png;
            if (img == null)
            {
                string imagePath = "C:\\Users\\daia_\\Desktop\\TRABALHOS DAIANE - CINEL\\Rafael\\AppCountriesExplorerWpf\\AppCountriesExplorerWpf\\Flags\\imagemindisponivel.png";

                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                    ImageFlagSelected.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                using (var foto = new WebClient())
                {
                    imageData = foto.DownloadData(img);
                }

                MemoryStream ms = new MemoryStream(imageData);

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                ImageFlagSelected.Source = image;
            }
        }
        /// <summary>
        /// Representa o manipulador de evento SelectionChanged do ComboBoxNameCountries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxNameCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var country = (Country)(ComboBoxNameCountries.SelectedItem);
            if (ComboBoxNameCountries != null && ComboBoxNameCountries.SelectedItem != null)
            {
                if (country.name.common == null)
                {
                    LabelName.Content = "N/D";

                }
                else
                {
                    LabelName.Content = country.name.common;
                }
                if (country.capital == null)
                {
                    LabelCapital.Content = "N/D";

                }
                else
                {
                    LabelCapital.Content = "";
                    foreach (string cap in country.capital)
                    {

                        LabelCapital.Content = LabelCapital.Content + "-" + cap;

                    }

                }
                if (country.region == null)
                {
                    LabelRegion.Content = "N/D";
                }
                else
                {
                    LabelRegion.Content = country.region;
                }
                if (country.subregion == null)
                {
                    LabelSubRegion.Content = "N/D";


                }
                else
                {
                    LabelSubRegion.Content = country.subregion;

                }
                if (country.population == 0)
                {
                    LabelPopulation.Content = "N/D";


                }
                else
                {
                    LabelPopulation.Content = country.population.ToString();

                }
                var giniStrings = country.gini?.Select(x => $"{x.Value} ({x.Key})") ?? Enumerable.Empty<string>();
                string giniString = string.Join(Environment.NewLine, giniStrings);
                if (country.gini.Count == 0)
                {
                    LabelGini.Content = "N/D";


                }
                else
                {
                    LabelGini.Content = giniString;

                }
                if (country.flags.png == null)
                {
                    LabelFlag.Content = "N/D";
                }
                else
                {
                    LabelFlag.Content = country.flags.png;
                }
                var connection = networkService.CheckConnection();
                if (connection.IsSucess)
                {
                    ShowFlag();


                }
                else
                {
                    ShowLocalFlag();
                    ImageFlagSelected.Source = country.image;
                }
            }

        }

        /// <summary>
        ///  Método responsável por exibir a bandeira do país selecionado caso não haja conexão com a internet.
        /// </summary>
        private void ShowLocalFlag()
        {
            var country = (Country)ComboBoxNameCountries.SelectedItem;
            ImageFlagSelected.Source = country.image;

        }
        /// <summary>
        /// Método responsável pelo download da bandeiras na pasta Flags criada no projeto.
        /// </summary>
        private async void DownloadFlags()
        {
            foreach (Country country in countries)
            {
                string url = country.flags.png;
                string pastaDestino = "C:\\Users\\daia_\\Desktop\\TRABALHOS DAIANE - CINEL\\Rafael\\AppCountriesExplorerWpf\\AppCountriesExplorerWpf\\Flags\\";


                try
                {
                    // Extrai o nome do arquivo da URL
                    string nomeArquivo = country.name.common + ".png";
                    // Define o caminho completo de destino
                    string caminhoDestino = System.IO.Path.Combine(pastaDestino, nomeArquivo);

                    using (HttpClient client = new HttpClient())
                    {
                        // Faz o download da imagem como um array de bytes
                        byte[] imagemBytes = await client.GetByteArrayAsync(url);

                        // Salva o array de bytes como um arquivo no caminho de destino
                        File.WriteAllBytes(caminhoDestino, imagemBytes);

                        MessageBox.Show("Image downloaded and saved successfully!!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro dowloading flags: " + ex.Message);
                }


            }
        }
        /// <summary>
        /// Representa o manipulador de evento ValueChanged do ProgressBarStatus. 
        /// Ele é executado sempre que o valor da barra de progresso é alterado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double progress = e.NewValue;

            if (progress >= 100)
            {

                ProgressBarStatus.ValueChanged -= ProgressBar_ValueChanged;
                ImageFlagsLoading.Visibility = Visibility.Collapsed;
            }
            else
            {
                await ProgressFlags();
            }
        }

        /// <summary>
        /// Método que atribui um evento de texto alterado a um TextBox dentro do ComboBoxNameCountries.
        /// </summary>
        private void AssignEventComboBox()
        {

            var textBox = ComboBoxNameCountries.Template.FindName("PART_EditableTextBox", ComboBoxNameCountries) as TextBox;
            if (textBox != null)
            {
                textBox.TextChanged += TextBox_TextChanged;

            }

        }
        /// <summary>
        /// Representa o manipulador de evento TextChanged para um TextBox dentro do ComboBoxNameCountries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string searchText = textBox.Text;

            ICollectionView view = CollectionViewSource.GetDefaultView(ComboBoxNameCountries.Items);
            view.Filter = item =>
            {
                if (string.IsNullOrEmpty(searchText))
                {
                    return true; // Mostrar todos os itens se o texto de pesquisa estiver vazio
                }

                // Realize o filtro com base no critério desejado, neste exemplo, verifica-se se o item contém o texto de pesquisa
                return item.ToString().Contains(searchText);

            };

            // Reatribuir a lista filtrada de itens à ComboBox

            ComboBoxNameCountries.IsDropDownOpen = true;
            return;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AppCountriesExplorerWpf.Model.Services
{
    /// <summary>
    /// A classe DataService lida com a interação com um banco de dados SQLite para salvar, 
    /// recuperar e excluir dados relacionados a países.
    /// </summary>
    public class DataService
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private DialogService dialogService;

        /// <summary>
        /// Construtor da classe DataService Verifica se o diretório "Data" existe e o cria se necessário.
        /// </summary>
        public DataService()
        {
            dialogService = new DialogService();
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }
            // Define o caminho da base de dados SQLite.
            var path = @"Data\Countries.sqlite";
            try
            {
                // Abre a conexão com o base de dados .
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                // Cria a tabela "Countries" se ela não existir.
                string sqlCommand = "Create table if not exists Countries(name varchar(250), capital varchar(250), region varchar(250), subregion varchar(250), population int, ginikey text, ginivalue real, flags varchar(250), imagem BLOB)";
                command = new SQLiteCommand(sqlCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                // Exibe uma mensagem de erro em caso de falha na criação da base de dados.
                dialogService.ShowMessage("Error!Unable to create the database", e.Message);
            }
        }
        /// <summary>
        ///  Salva os dados dos países na base de dados.
        /// </summary>
        /// <param name="countries"></param>
        public void SaveData(List<Country> countries)
        {
            try
            {
                foreach (var country in countries)
                {

                    if (country.capital == null)
                    {
                        country.capital = new List<string>();
                    }
                    string aux = "";
                    foreach (string cap in country.capital)
                    {
                        aux = aux + cap + '#';
                    }
                    if (country.gini == null)
                    {
                        country.gini = new Dictionary<string, double>();
                    }
                    // Baixa a imagem da bandeira do país a partir da URL especificada.
                    byte[] imageData;
                    using (var foto = new WebClient())
                    {
                        imageData = foto.DownloadData(country.flags.png);
                    }

                    MemoryStream ms = new MemoryStream(imageData);

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = ms;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    // Prepara e executa o comando SQL para inserir os dados do país na base de dados.
                    string sql = "Insert into Countries(name, capital, region, subregion, population, ginikey, ginivalue, flags, imagem) values ( @name, @capital, @region, @subregion, @population, @ginikey, @ginivalue, @flags, @imagem)";
                    command = new SQLiteCommand(sql, connection);
                    command.Parameters.AddWithValue("@name", country.name.common);
                    command.Parameters.AddWithValue("@capital", aux);
                    command.Parameters.AddWithValue("@region", country.region);
                    command.Parameters.AddWithValue("@subregion", country.subregion);
                    command.Parameters.AddWithValue("@population", country.population);
                    command.Parameters.AddWithValue("@ginikey", country.gini?.Keys.FirstOrDefault());
                    command.Parameters.AddWithValue("@ginivalue", country.gini?.Values.FirstOrDefault());
                    command.Parameters.AddWithValue("@flags", country.flags.png);
                    command.Parameters.AddWithValue("@imagem", imageData);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error! Unable to save data into database", e.Message);
            }
        }
        /// <summary>
        /// Obtém os dados dos países a partir da base de dados.
        /// </summary>
        /// <returns></returns>
        public List<Country> GetData()
        {
            List<Country> countries = new List<Country>();
            try
            {
                string sql = "select name, capital, region, subregion, population, ginikey, ginivalue, flags, imagem from Countries";
                command = new SQLiteCommand(sql, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                // Verifica e trata os valores nulos dos campos antes de atribuí-los às propriedades do objeto Country.
                while (reader.Read())
                {
                    var country = new Country();
                    if (!Convert.IsDBNull(reader["name"]))
                    {
                        Name name = new Name();
                        name.common = (string)reader["name"];
                        country.name = name;
                    }
                    if (!Convert.IsDBNull(reader["capital"]))
                    {
                        country.capital = new List<string>(reader["capital"].ToString().Split('#'));
                    }
                    if (!Convert.IsDBNull(reader["region"]))
                    {
                        country.region = (string)reader["region"];
                    }
                    if (!Convert.IsDBNull(reader["subregion"]))
                    {
                        country.subregion = (string)reader["subregion"];
                    }
                    if (!Convert.IsDBNull(reader["population"]))
                    {
                        country.population = (int)reader["population"];
                    }
                    if (!Convert.IsDBNull(reader["flags"]))
                    {
                        Flag flag = new Flag();
                        flag.png = (string)reader["flags"];
                        country.flags = flag;
                    }
                    if (!Convert.IsDBNull(reader["imagem"]))
                    {
                        byte[] imageData = (byte[])reader["imagem"];

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(imageData);
                        bitmap.EndInit();

                        country.image = bitmap;
                    }
                    object giniKey = reader["Ginikey"];
                    object giniValue = reader["Ginivalue"];

                    if (!Convert.IsDBNull(giniKey) && !Convert.IsDBNull(giniValue))
                    {
                        country.gini = new Dictionary<string, double> { { (string)giniKey, (double)giniValue } };



                    }
                    //Adiciona o objeto Country à lista de países.
                    countries.Add(country);


                }
                connection.Close();
                return countries;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error! Unable to get the data from database!!!", e.Message);
                return null;
            }
        }
        /// <summary>
        /// // Exclui todos os dados da tabela Countries na base de dados.
        /// </summary>
        public void DeleteData()
        {
            try
            {
                string sql = "Delete from Countries";
                command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error! Unable to delete the data from database!!!", e.Message);
            }
        }
    }
}

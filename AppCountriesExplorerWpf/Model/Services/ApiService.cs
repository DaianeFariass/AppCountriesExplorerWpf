using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppCountriesExplorerWpf.Model.Services
{
    public class ApiService
    {
        // Obtém a lista de países da API usando o HttpClient.
        public async Task<Response> GetCountries(string urlBaseApi, string controllerApi)
        {

            try
            {
                var client = new HttpClient(); //Criar o HTTPCLIENT para fazer a ligação 
                client.BaseAddress = new Uri(urlBaseApi); // Inserir o endereço base da API
                var response = await client.GetAsync(controllerApi);
                var result = await response.Content.ReadAsStringAsync(); // Agarro no que ela devolve e guarda no result

                if (!response.IsSuccessStatusCode) // Se não correu bem passa por aqui
                {
                    return new Response
                    {
                        IsSucess = false,
                        Message = result
                    };
                }

                //Converte o resultado da API em uma lista de objetos Country usando o JsonConvert.
                var obj = JsonConvert.DeserializeObject<List<Country>>(result);

                //Retorna uma resposta de sucesso com a lista de países como resultado.
                return new Response
                {
                    IsSucess = true,
                    Result = obj
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucess = false,
                    Message = ex.Message
                };


            }
        }
    }
}

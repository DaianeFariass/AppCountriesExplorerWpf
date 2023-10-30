using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AppCountriesExplorerWpf.Model.Services
{
    public class NetworkService
    {
        /// <summary>
        /// Cria uma instância do WebClient para solicitação HTTP.
        /// </summary>
        /// <returns></returns>
        public Response CheckConnection()
        {
            var client = new WebClient();
            try
            {
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return new Response
                    {
                        IsSucess = true,
                    };
                }

            }
            catch (Exception)
            {
                return new Response
                {
                    IsSucess = false,
                    Message = "Configure your internet connection!!!"

                };

            }
        }
    }
}

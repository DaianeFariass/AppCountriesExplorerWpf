using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCountriesExplorerWpf.Model
{
    /// <summary>
    /// // A classe Response representa uma resposta ou resultado de uma operação.
    /// </summary>
    public class Response
    {
        public bool IsSucess { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
    }
}

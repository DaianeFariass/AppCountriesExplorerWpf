using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AppCountriesExplorerWpf.Model.Services
{
    /// <summary>
    /// // A classe DialogService fornece métodos para exibir caixas de diálogo de mensagem.
    /// </summary>
    public class DialogService
    {
        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(title, message);
        }
    }
}

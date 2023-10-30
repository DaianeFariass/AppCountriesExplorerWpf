using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AppCountriesExplorerWpf.Model
{
    /// <summary>
    /// A classe Country representa as informações de um país.
    /// </summary>
    public class Country
    {
        public Name name { get; set; }

        public List<string> capital { get; set; }

        public string region { get; set; }

        public string subregion { get; set; }

        public int population { get; set; }

        public Dictionary<string, double> gini { get; set; }

        public Flag flags { get; set; }

        public string ImagePath { get; set; }

        public Map maps { get; set; }

        public BitmapImage image { get; set; }


        public override string ToString()
        {
            return $"{name} - {capital} - {region} - {subregion} - {population} - {gini} - {flags} - {ImagePath} - {maps} - {image}";
        }
    }
    public class Name
    {
        public string common { get; set; }

    }
    public class Flag
    {
        public string png { get; set; }
    }
    public class Map
    {
        public string openStreetMaps { get; set; }
    }

}


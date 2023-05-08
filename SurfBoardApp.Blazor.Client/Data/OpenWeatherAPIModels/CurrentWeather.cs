using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Blazor.Client.Data.OpenWeatherAPIModels
{
    public class CurrentWeather
    {
        public int id { get; set; }
        public int cod { get; set; }
        public int visibility { get; set; }
        public int timezone { get; set; }
        public string name { get; set; }
        public int dt { get; set; }
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public Main main { get; set; }
        public Sys sys { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Data.OpenWeatherAPIModels
{
    public class WeatherForecast
    {
        public string cod { get; set; }
        public int message { get; set; }
        public int cnt { get; set; }
        public City city { get; set; }
        public List<List> list { get; set; }
    }
}

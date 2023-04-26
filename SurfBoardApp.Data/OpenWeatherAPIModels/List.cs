using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Data.OpenWeatherAPIModels
{
    public class List
    {
        public int dt { get; set; }
        public double pop { get; set; }
        public int visibility { get; set; }
        public string? dt_text { get; set; }
        public Main main { get; set; }
        public List<Weather> weather { get; set; }
        public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        public Sys sys { get; set; }
        public Rain rain { get; set; }

    }
}

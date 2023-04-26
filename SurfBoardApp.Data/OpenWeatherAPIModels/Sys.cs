using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Data.OpenWeatherAPIModels
{
    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string? country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
        public string? pod { get; set; }
    }
}

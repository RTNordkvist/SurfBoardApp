using Microsoft.AspNetCore.Http;
using SurfBoardApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels
{
    public class BoardVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Thickness { get; set; }
        public double? Volume { get; set; }
        public string? Type { get; set; }
        public decimal? Price { get; set; }
        public string? Equipment { get; set; }
        public List<Image>? Images { get; set; }
    }
}

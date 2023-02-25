using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.ViewModels
{
    public class BoardModel
    {
        [Required]
        [MinLength(1, ErrorMessage = "The board name must be at least 1 character long")]
        public string? Name { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Thickness { get; set; }
        public double? Volume { get; set; }
        public string? Type { get; set; }

        [Required]
        [Range(0.0, (double)decimal.MaxValue, ErrorMessage = "The price must be a positive value")]
        public decimal? Price { get; set; }
        public string? Equipment { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}

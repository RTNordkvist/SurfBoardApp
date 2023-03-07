using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.Models
{
    public class Board
    {

        public int Id { get; set; }

        [Required]
        [MinLength(1, ErrorMessage ="The board name must be at least 1 character long")]
        public string? Name { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "The length must be a positive value")]
        public double? Length { get; set; }
        
        [Range(0.0, double.MaxValue, ErrorMessage = "The width must be a positive value")]
        public double? Width { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "The thickness must be a positive value")]
        public double? Thickness { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "The volume must be a positive value")]
        public double? Volume { get; set; }
        
        public string? Type { get; set; }
        
        [Required]
        [Range(0.0, (double)decimal.MaxValue, ErrorMessage="The price must be a positive value")]
        public decimal? Price { get; set; }
        
        public string? Equipment { get; set; }
        
        public List<Image>? Images { get; set; }

        public List<Booking>? RentPeriods { get; set; }

        public int? ClickCount { get; set; }

        public void RemoveImage(Image image)
        {
            if (Images != null && Images.Contains(image))
            {
                Images.Remove(image);
            }
        }

    }

}

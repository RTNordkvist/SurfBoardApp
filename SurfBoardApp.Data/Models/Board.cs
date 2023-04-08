using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.Data.Models
{
    public class Board
    {

        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        // [MaxLength(255)]
        public string Name { get; set; }

        [Range(0.0, double.MaxValue)]
        public double? Length { get; set; }

        [Range(0.0, double.MaxValue)]
        public double? Width { get; set; }

        [Range(0.0, double.MaxValue)]
        public double? Thickness { get; set; }

        [Range(0.0, double.MaxValue)]
        public double? Volume { get; set; }

        public string? Type { get; set; }

        [Required]
        [Range(0.0, (double)decimal.MaxValue)]
        public decimal Price { get; set; }

        public string? Equipment { get; set; }

        public List<Image>? Images { get; set; }

        public List<Booking>? Bookings { get; set; }

        public int? ClickCount { get; set; }

        public int Version { get; set; }

        public bool MembersOnly { get; set; }
    }

}

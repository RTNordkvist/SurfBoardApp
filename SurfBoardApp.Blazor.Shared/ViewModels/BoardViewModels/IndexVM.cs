using SurfBoardApp.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels
{
    public class IndexVM
    {
        public PaginatedList<IndexBoardVM>? Boards { get; set; }

        public string? SearchString { get; set; }

        public int? PageNumber { get; set; }

        public int PageSize { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BookingStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BookingEndDate { get; set; }


        //Testing Area
        // Added properties
        public string? SearchParameter { get; set; }
        public string? SearchValue { get; set; }

        public double? SearchLength { get; set; }
        public double? SearchWidth { get; set; }
        public double? SearchThickness { get; set; }
        public double? SearchVolume { get; set; }
        public string? SearchType { get; set; }
        public decimal? SearchPrice { get; set; }
    }
}

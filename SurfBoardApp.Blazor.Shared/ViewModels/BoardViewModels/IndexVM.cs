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

        public double? SearchLengthFrom { get; set; }
        public double? SearchLengthTo { get; set; }

        public double? SearchWidthFrom { get; set; }
        public double? SearchWidthTo { get; set; }

        public double? SearchThicknessFrom { get; set; }
        public double? SearchThicknessTo { get; set; }

        public double? SearchVolumeFrom { get; set; }
        public double? SearchVolumeTo { get; set; }

        public bool IncludeEquipment { get; set; }
    }
}

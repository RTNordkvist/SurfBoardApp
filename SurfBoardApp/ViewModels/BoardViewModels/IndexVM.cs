﻿using SurfBoardApp.Models;
using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.ViewModels.BoardViewModels
{
    public class IndexVM
    {
        public PaginatedList<Board>? Boards { get; set; }

        public string? SearchString { get; set; }

        public int? PageNumber { get; set; }

        public int PageSize { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BookingStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BookingEndDate { get; set; }

        public bool ShowBookingOptions { get; set; }
    }
}
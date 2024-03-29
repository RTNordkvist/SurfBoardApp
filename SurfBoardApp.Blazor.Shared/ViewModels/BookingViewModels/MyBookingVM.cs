﻿using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.Blazor.Shared.ViewModels.BookingViewModels
{
    public class MyBookingVM
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string BoardName { get; set; }

        public int BoardId { get; set; }

        public int BookingId { get; set; }
    }
}

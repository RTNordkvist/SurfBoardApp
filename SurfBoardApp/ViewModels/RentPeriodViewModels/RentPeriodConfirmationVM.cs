﻿using SurfBoardApp.Models;
using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.ViewModels.RentPeriodViewModels
{
    public class RentPeriodConfirmationVM
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string BoardName { get; set; }
    }
}

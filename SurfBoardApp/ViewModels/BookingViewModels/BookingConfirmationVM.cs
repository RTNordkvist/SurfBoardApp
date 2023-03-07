using SurfBoardApp.Models;
using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.ViewModels.BookingViewModels
{
    public class BookingConfirmationVM
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string BoardName { get; set; }
    }
}

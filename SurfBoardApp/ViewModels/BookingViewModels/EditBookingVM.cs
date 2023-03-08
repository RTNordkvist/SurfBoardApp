using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.ViewModels.BookingViewModels
{
    public class EditBookingVM
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string BoardName { get; set; }
    }
}

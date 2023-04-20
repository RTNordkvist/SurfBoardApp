using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.Blazor.Shared.ViewModels.BookingViewModels
{
    public class EditBookingVM
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        [DateGreaterThan("StartDate", ErrorMessage = "End Date must be after Start Date")]
        public DateTime EndDate { get; set; }

        public string? BoardName { get; set; }
    }
}

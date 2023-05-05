using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Blazor.Shared.ViewModels.BookingViewModels
{
    public class CreateBookingVM
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BoardId { get; set; }
        public string? NonUserEmail { get; set; }
    }
}

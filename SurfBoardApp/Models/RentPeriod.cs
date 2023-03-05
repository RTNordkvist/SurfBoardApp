using Microsoft.AspNetCore.Identity;

namespace SurfBoardApp.Models
{
    public class RentPeriod
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CustomerId { get; set; }
        public IdentityUser Customer { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; }
    }
}

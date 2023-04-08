using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp.Data.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? CustomerId { get; set; }
        public ApplicationUser? Customer { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; }
        public string? NonUserEmail { get; set; }
    }
}

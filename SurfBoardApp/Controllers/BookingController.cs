using SurfBoardApp.Data;
using SurfBoardApp.Models;
using SurfBoardApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurfBoardApp.ViewModels.BookingViewModels;

namespace SurfBoardApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly SurfBoardAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager; 
        public BookingController(SurfBoardAppContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var bookings = await _context.Booking.Include(x=> x.Board).Where(b => b.CustomerId == user.Id).ToListAsync();
            var bookingViewModels = new List<MyBookingVM>();
            foreach (var booking in bookings)
            {
                var bookingVM = new MyBookingVM()
                {
                    StartDate = booking.StartDate,
                    EndDate = booking.EndDate,
                    BoardId = booking.BoardId,
                    BoardName = booking.Board.Name
                };
                bookingViewModels.Add(bookingVM);
            }
            return View(bookingViewModels);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> BookBoard(DateTime startDate, DateTime endDate, int boardId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var board = await _context.Board.FindAsync(boardId); if (board == null)
            {
                return NotFound($"Unable to load board with ID '{boardId}'.");
            }
            if (startDate >= endDate)
            {
                ModelState.AddModelError(string.Empty, "The start date must be before the end date.");
                return RedirectToAction("Index");
            }
            var existingBooking = await _context.Booking.FirstOrDefaultAsync(b => b.BoardId == boardId && b.EndDate > startDate && b.StartDate < endDate); if (existingBooking != null)
            {
                ModelState.AddModelError(string.Empty, $"The board '{board.Name}' is not available during the selected period.");
                return RedirectToAction("Index");
            }
            var booking = new Booking
            {
                CustomerId = user.Id,
                BoardId = boardId,
                StartDate = startDate,
                EndDate = endDate
            }; _context.Booking.Add(booking);
            await _context.SaveChangesAsync(); return RedirectToAction("Index");
        }
    }
}

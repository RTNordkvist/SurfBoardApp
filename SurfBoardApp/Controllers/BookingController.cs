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

        public IActionResult EditBooking(string boardName, DateTime startDate, DateTime endDate)
        {
            var model = new EditBookingVM
            {
                BoardName = boardName,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(model);
        }

        
        [HttpPost]
        public IActionResult EditBooking(EditBookingVM model)
        {
            if (ModelState.IsValid)
            {
                var booking = _context.Booking.SingleOrDefault(b => b.StartDate == model.StartDate && b.EndDate == model.EndDate);

                if (booking != null)
                {
                    booking.StartDate = model.StartDate;
                    booking.EndDate = model.EndDate;

                    _context.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "Booking not found.");
                }
            }

            return RedirectToAction("MyBookings");
        }


        // Delete booking
        public async Task<IActionResult> DeleteBooking(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var board = await _context.Booking
                .FirstOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(MyBookings);
        }

        // Delete booking
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Booking == null)
            {
                return Problem("Entity set 'SurferDemoContext.Booking'  is null.");
            }
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MyBookings");
        }



    }
}

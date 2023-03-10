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
        // BookingController constructor with required dependencies
        private readonly SurfBoardAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(SurfBoardAppContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action method that displays bookings of the authenticated user
        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            // Get the user object of the authenticated user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Get all bookings of the authenticated user and include board information
            var bookings = await _context.Booking.Include(x => x.Board).Where(b => b.CustomerId == user.Id).ToListAsync();

            // Create a view model for each booking and add it to a list
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

            // Return a view that displays the list of booking view models
            return View(bookingViewModels);
        }


        // This method is called when the user wants to edit a booking. It receives the board name, 
        // start date and end date as parameters and returns the EditBooking view with a new 
        // EditBookingVM object as a model.
        public IActionResult EditBooking(string boardName, DateTime startDate, DateTime endDate)
        {
            // Creates a new EditBookingVM object with the data passed by the user and the original 
            // start and end dates of the booking.
            var model = new EditBookingVM
            {
                BoardName = boardName,
                StartDate = startDate,
                EndDate = endDate,
                OriginalStartDate = startDate,
                OriginalEndDate = endDate
            };

            // Returns the EditBooking view with the model.
            return View(model);
        }

        // This method is called when the user submits the edit form. It receives an EditBookingVM 
        // object as a parameter with the updated data and updates the corresponding booking in the 
        // database.
        [HttpPost]
        public IActionResult EditBooking(EditBookingVM model)
        {
            if (ModelState.IsValid)
            {
                // Searches for the booking in the database based on the original start and end dates.
                var booking = _context.Booking.SingleOrDefault(b => b.StartDate == model.OriginalStartDate && b.EndDate == model.OriginalEndDate);

                if (booking != null)
                {
                    // If the booking was found and the start or end dates have changed, updates the 
                    // booking in the database and saves the changes.
                    if (booking.StartDate != model.StartDate || booking.EndDate != model.EndDate)
                    {
                        booking.StartDate = model.StartDate;
                        booking.EndDate = model.EndDate;
                        _context.SaveChanges();
                    }
                }
                else
                {
                    // If the booking was not found, adds an error to the ModelState.
                    ModelState.AddModelError("", "Booking not found.");
                }
            }

            // Redirects the user to the MyBookings action.
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

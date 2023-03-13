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
                    BoardName = booking.Board.Name,
                    BookingId = booking.Id
                };
                bookingViewModels.Add(bookingVM);
            }

            // Return a view that displays the list of booking view models
            return View(bookingViewModels);
        }


        // This method is called when the user wants to edit a booking. It receives the board name, 
        // start date and end date as parameters and returns the EditBooking view with a new 
        // EditBookingVM object as a model.
        public async Task<IActionResult> EditBooking(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }
            var booking = await _context.Booking.Include(x => x.Board).FirstOrDefaultAsync(x => x.Id == id);
            if (booking == null)
            {
                return NotFound();
            }


            // Creates a new EditBookingVM object with the data passed by the user and the original 
            // start and end dates of the booking.
            var model = new EditBookingVM
            {
                Id = booking.Id,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                BoardName = booking.Board.Name
            };

            // Returns the EditBooking view with the model.
            return View(model);
        }

        // This method is called when the user submits the edit form. It receives an EditBookingVM 
        // object as a parameter with the updated data and updates the corresponding booking in the 
        // database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditBooking(EditBookingVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

                // Searches for the booking in the database based on the original start and end dates.
                var booking = _context.Booking.FirstOrDefault(b => b.Id == model.Id);

                if (booking != null)
                {
                        booking.StartDate = model.StartDate;
                        booking.EndDate = model.EndDate;
                        await _context.SaveChangesAsync();
                }
                else
                {
                    // If the booking was not found, adds an error to the ModelState.
                    ModelState.AddModelError("", "Booking not found.");
                }


            // Redirects the user to the MyBookings action.
            return RedirectToAction("MyBookings");
        }


        [Authorize]
        public async Task<IActionResult> DeleteBooking(int? id)
        {
            var booking = await _context.Booking.FirstOrDefaultAsync(b => b.Id == id );

            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyBookings");
        }

        // Delete booking
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Booking == null)
            {
                return Problem("Entity set 'SurferDemoContext.Booking'  is null.");
            }

            var booking = await _context.Booking.FirstOrDefaultAsync(b => b.Id == id);

            if (booking != null)
            {
                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyBookings");
        }





    }
}

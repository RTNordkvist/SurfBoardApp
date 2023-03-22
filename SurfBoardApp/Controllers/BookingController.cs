using SurfBoardApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfBoardApp.Data.Models;
using SurfBoardApp.Blazor.Shared.ViewModels.BookingViewModels;
using SurfBoardApp.Blazor.Shared.ViewModels.BoardViewModels;
using SurfBoardApp.Domain.Exceptions;
using SurfBoardApp.Domain.Services;

namespace SurfBoardApp.Controllers
{
    public class BookingController : BaseController
    {
        // BookingController constructor with required dependencies
        private readonly BookingService _bookingService;
        private readonly BoardService _boardService;

        public BookingController(UserManager<ApplicationUser> userManager, BookingService bookingService, BoardService boardService) : base(userManager)
        {
            _bookingService = bookingService;
            _boardService = boardService;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookBoard(BookBoardVM model)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index), await _boardService.GetBoardModels(new IndexVM { BookingStartDate = model.StartDate, BookingEndDate = model.EndDate })); //TODO is this is what is supposed to happen?

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("InvalidEndDate", "End date cannot be before start date");
                return View(nameof(Index), await _boardService.GetBoardModels(new IndexVM { BookingStartDate = model.StartDate, BookingEndDate = model.EndDate })); //TODO unittest
            }

            try
            {
                var result = await _bookingService.AddBooking(model);
                return View(result);
            }
            catch (UnavailableBookingException)
            {
                ModelState.AddModelError("BoardUnavailable", "Board is unavailable for the selected period");
                return View(nameof(Index), await _boardService.GetBoardModels(new IndexVM { BookingStartDate = model.StartDate, BookingEndDate = model.EndDate }));
            }
        }

        //TODO Refactor from here and below -> move responsibility to BookingService

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

            var bookingViewModels = await _bookingService.GetCustomerBookings();

            // Return a view that displays the list of booking view models
            return View(bookingViewModels);
        }


        // This method is called when the user wants to edit a booking. It receives the board name, 
        // start date and end date as parameters and returns the EditBooking view with a new 
        // EditBookingVM object as a model.
        public async Task<IActionResult> EditBooking(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _bookingService.GetEditBooking((int)id);

            if (result == null)
            {
                return NotFound();
            }

            // Returns the EditBooking view with the model.
            return View(result);
        }

        // This method is called when the user submits the edit form. It receives an EditBookingVM 
        // object as a parameter with the updated data and updates the corresponding booking in the 
        // database.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBooking(EditBookingVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _bookingService.UpdateBookingDates(model);
            }
            catch (BookingNotFoundException)
            {
                // If the booking was not found, adds an error to the ModelState.
                ModelState.AddModelError("", "Booking not found.");
                return View(model);
            }

            // Redirects the user to the MyBookings action.
            return RedirectToAction("MyBookings");
        }


        [Authorize]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                await _bookingService.RemoveBooking(id);

                // Redirects the user to the MyBookings action.
                return RedirectToAction("MyBookings");
            }
            catch (BookingNotFoundException)
            {
                // If the booking was not found, adds an error to the ModelState.
                ModelState.AddModelError("", "Booking not found.");
                return NotFound();
            }
        }
    }
}

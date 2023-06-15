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
    public class BookingController : Controller
    {
        // BookingController constructor with required dependencies
        private readonly BookingService _bookingService;
        private readonly BoardService _boardService;

        public BookingController(BookingService bookingService, BoardService boardService)
        {
            _bookingService = bookingService;
            _boardService = boardService;
        }

        // GET: Booking/Create
        public IActionResult CreateBooking(int boardId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var model = new CreateBookingVM()
            {
                BoardId = boardId,
                StartDate = startDate.HasValue ? startDate.Value : DateTime.UtcNow.Date,
                EndDate = endDate.HasValue ? endDate.Value : DateTime.UtcNow.Date.AddDays(3)
            };

            return View(model);
        }

        // POST: Boards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(CreateBookingVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError(nameof(CreateBookingVM.EndDate), "End date cannot be before start date");
                return View(nameof(CreateBooking), new CreateBookingVM() { BoardId = model.BoardId, StartDate = model.StartDate, EndDate = model.EndDate, NonUserEmail = model.NonUserEmail}); //TODO unittest
            }

            try
            {
                var result = await _bookingService.AddBooking(model);
                return View("BookingConfirmation", result);
            }
            catch (UnavailableBookingException)
            {
                ModelState.AddModelError(nameof(CreateBookingVM.EndDate), "Board is unavailable for the selected period");
                return View(nameof(CreateBooking), new CreateBookingVM() { BoardId = model.BoardId, StartDate = model.StartDate, EndDate = model.EndDate, NonUserEmail = model.NonUserEmail }); //TODO unittest
            }
            catch (NoOfBookingsLimitReachedException)
            {
                ModelState.AddModelError(nameof(CreateBookingVM.EndDate), "You have reached your limit for making bookings without being registered");
                return View(nameof(CreateBooking), new CreateBookingVM() { BoardId = model.BoardId, StartDate = model.StartDate, EndDate = model.EndDate, NonUserEmail = model.NonUserEmail }); //TODO unittest
            }
        }

        // Action method that displays bookings of the authenticated user
        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
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

            try
            {
                var result = await _bookingService.GetEditBooking((int)id);

                // Returns the EditBooking view with the model.
                return View(result);
            }
            catch (BookingNotFoundException)
            {
                return NotFound();
            }
        }


        //ModelState en ordbog, der indeholder både data indsendt af brugeren og eventuelle valideringsfejl
        //genereret under modelbinding.
        //Det bruges til at kommunikere valideringsfejl tilbage til viewet, så brugeren kan rette dem.

        [HttpPost] // Only accept HTTP POST requests
        [Authorize] // Requires the user to be authenticated
        [ValidateAntiForgeryToken] // Protects against cross-site request forgery attacks
        public async Task<IActionResult> EditBooking(EditBookingVM model)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is invalid, return the same view with the invalid model to display validation errors
                return View(model);
            }

            // Attempt to update the booking dates using the BookingService
            var (success, message, nextAvailableDate) = await _bookingService.UpdateBookingDates(model);

            if (!success)
            {
                // If the booking dates were not updated, add an error message to the model state
                if (nextAvailableDate != null)
                {
                    ModelState.AddModelError("", $"{message} The next available date is {nextAvailableDate?.ToString("yyyy-MM-dd")}");
                }
                else
                {
                    ModelState.AddModelError("", message);
                }

                // Return the same view with the invalid model to display the error message
                return View(model);
            }

            // If the booking dates were successfully updated, redirect the user to the MyBookings action
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

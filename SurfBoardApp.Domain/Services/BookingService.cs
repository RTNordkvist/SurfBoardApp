using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurfBoardApp.Blazor.Shared.ViewModels.BookingViewModels;
using SurfBoardApp.Data;
using SurfBoardApp.Data.Models;
using SurfBoardApp.Domain.Exceptions;

namespace SurfBoardApp.Domain.Services
{
    public class BookingService
    {
        private readonly SurfBoardAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookingService(SurfBoardAppContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<EditBookingVM> GetEditBooking(int id)
        {
            var booking = await _context.Booking
                .Include(x => x.Board)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (booking == null)
            {
                throw new BookingNotFoundException();
            }

            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            if (userId != booking.CustomerId)
            {
                throw new BookingNotFoundException(); // For security reasons the system don't reveal that the booking was actually found but doesn't belong to the user
            }

            // Creates a new EditBookingVM object with the data passed by the user and the original 
            // start and end dates of the booking.
            var model = new EditBookingVM
            {
                Id = booking.Id,
                BoardName = booking.Board.Name,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate
            };

            return model;
        }

        public async Task<List<MyBookingVM>> GetCustomerBookings()
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            // Get all bookings of the authenticated user and include board information
            var bookingViewModels = await _context.Booking
                .Include(x => x.Board)
                .Where(b => b.CustomerId == userId)
                // Project to a view model for each booking and add it to a list
                .Select(x => new MyBookingVM()
                {
                    BookingId = x.Id,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    BoardId = x.BoardId,
                    BoardName = x.Board.Name
                })
                .ToListAsync();

            return bookingViewModels;
        }

        public async Task<BookingConfirmationVM> AddBooking(CreateBookingVM model)
        {
            string? userId = null;
            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            }

            // Check if the board is available, if not - throw an exception
            if (await _context.Booking.AnyAsync(x => x.StartDate <= model.EndDate && x.EndDate >= model.StartDate && x.BoardId == model.BoardId))
            {
                throw new UnavailableBookingException();
            }

            // The viewmodel is projected to a booking entity model
            var booking = new Booking
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                BoardId = model.BoardId,
                CustomerId = userId,
                NonUserEmail = model.NonUserEmail
            };

            // The new booking is updated in the database
            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            var bookingConfirmation = new BookingConfirmationVM
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                BoardName = _context.Board.AsNoTracking().First(x => x.Id == model.BoardId).Name
            };

            return bookingConfirmation;
        }

        //Success og Message er brugt i EditBooking i BookingController
        //success bruges variablen til at kontrollere, om reservationsopdateringen lykkedes eller ej,
        //og variablen message bruges til at vise fejlmeddelelser, hvis opdateringen mislykkedes.
        public async Task<(bool success, string message, DateTime? nextAvailableDate)> UpdateBookingDates(EditBookingVM model)
        {
            // Retrieve the booking with the given Id
            var booking = _context.Booking.FirstOrDefault(x => x.Id == model.Id);

            // Check if booking exists
            if (booking == null)
            {
                return (false, "Booking not found.", null);
            }

            // Get the user Id of the currently logged in user
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            // Check if the current user is authorized to update the booking
            if (userId != booking.CustomerId)
            {
                return (false, "You are not authorized to update this booking.", null);
            }

            // Check if the board is available for the new date range, if not - return an error message
            if (await _context.Booking.AnyAsync(x => x.StartDate <= model.EndDate && x.EndDate >= model.StartDate && x.BoardId == booking.BoardId && x.Id != booking.Id))
            {
                // Get the next available date for the board
                var nextAvailableDate = await _context.Booking
                    .Where(x => x.BoardId == booking.BoardId && x.EndDate < model.StartDate)
                    .OrderByDescending(x => x.EndDate)
                    .Select(x => x.EndDate)
                    .FirstOrDefaultAsync();

                // Return an error message with the next available date
                return (false, "The board is not available in the selected date.", null);
            }

            // Update the start and end date
            booking.StartDate = model.StartDate;
            booking.EndDate = model.EndDate;
            await _context.SaveChangesAsync();

            // Return success message
            return (true, "Booking dates updated successfully.", null);
        }



        /// <summary>
        /// Removes a booking based on the primary key (booking id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="BookingNotFoundException"></exception>
        public async Task RemoveBooking(int id)
        {
            // TODO add security check -> Only the user that has the booking should be able to edit it

            var booking = await _context.Booking.FirstOrDefaultAsync(x => x.Id == id);

            if (booking == null)
            {
                throw new BookingNotFoundException();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}

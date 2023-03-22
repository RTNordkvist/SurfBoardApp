using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurfBoardApp.Blazor.Shared.ViewModels.BookingViewModels;
using SurfBoardApp.Data;
using SurfBoardApp.Data.Models;
using SurfBoardApp.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfBoardApp.Domain.Services
{
    public class BookingService
    {
        private readonly SurfBoardAppContext _context; //DBcontext
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
            var booking = await _context.Booking.Include(x => x.Board).FirstOrDefaultAsync(x => x.Id == id);

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
            var bookings = await _context.Booking.Include(x => x.Board).Where(b => b.CustomerId == userId).ToListAsync();

            // Create a view model for each booking and add it to a list
            var bookingViewModels = new List<MyBookingVM>();
            foreach (var booking in bookings)
            {
                var bookingVM = new MyBookingVM()
                {
                    BookingId = booking.Id,
                    StartDate = booking.StartDate,
                    EndDate = booking.EndDate,
                    BoardId = booking.BoardId,
                    BoardName = booking.Board.Name
                };
                bookingViewModels.Add(bookingVM);
            }

            return bookingViewModels;
        }
        public async Task<BookingConfirmationVM> AddBooking(BookBoardVM model)
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            //if the endDate lies before the startDate, an exception is thrown
            if (await _context.Booking.AnyAsync(x => x.StartDate <= model.EndDate && x.EndDate >= model.StartDate && x.BoardId == model.BoardId))
            {
                throw new UnavailableBookingException();
            }

            //The viewmodel is projected to a booking entity model
            var booking = new Booking
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                BoardId = model.BoardId,
                CustomerId = userId
            };

            //The new booking is updated in the database
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

        public async Task<bool> UpdateBookingDates(EditBookingVM model)
        {
            // Searches for the booking in the database based on the original start and end dates.
            var booking = _context.Booking.FirstOrDefault(x => x.Id == model.Id);

            if (booking == null)
            {
                throw new BookingNotFoundException();
            }

            // If the booking was found and the start or end dates have changed, updates the 
            // booking in the database and saves the changes.
            booking.StartDate = model.StartDate;
            booking.EndDate = model.EndDate;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveBooking(int id)
        {
            var booking = await _context.Booking.FirstOrDefaultAsync(x => x.Id == id);

            if (booking == null)
            {
                throw new BookingNotFoundException();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

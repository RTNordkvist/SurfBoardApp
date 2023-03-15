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
        private readonly SurfBoardAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookingService(SurfBoardAppContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
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
    }
}

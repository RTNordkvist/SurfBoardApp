using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SurfBoardApp.Blazor.Shared.ViewModels.ApplicationUserViewModels;
using SurfBoardApp.Data.Models;
using SurfBoardApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SurfBoardApp.Domain.Services
{
    public class UserService
    {
        private readonly SurfBoardAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(SurfBoardAppContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserVM> GetCurrentUser()
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (currentUser == null)
            {
                return null;
            }

            var result = new UserVM()
            {
                UserId = currentUser.Id,
                UserName = currentUser.UserName,
                Roles = await _userManager.GetRolesAsync(currentUser)
            };

            return result;
        }
    }
}

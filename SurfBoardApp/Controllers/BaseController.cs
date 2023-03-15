using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurfBoardApp.Data.Models;

namespace SurfBoardApp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly UserManager<ApplicationUser> _userManager;

        public BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
    }
}

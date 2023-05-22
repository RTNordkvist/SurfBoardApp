using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurfBoardApp.Domain.Services;

namespace SurfBoardApp.Areas.api
{
    [Route("api/user/[action]")]

    public class UserApiController: Controller
    {
        private readonly UserService _userService;

        public UserApiController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Returns the current user including roles
        /// </summary>
        /// <returns>Null if no current user has been authenticated</returns>
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await _userService.GetCurrentUser();

            return Ok(result);
        }
    }
}

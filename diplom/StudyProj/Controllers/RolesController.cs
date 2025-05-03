using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace StudyProj.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RolesController : Controller
    {
        RoleManager<Role> _roleManager;
        UserManager<User> _userManager;
        public RolesController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            return new JsonResult(await _userManager.Users.Include(u => u.Facility).ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> RoleList()
        {
            return new JsonResult(await _roleManager.Roles.ToListAsync());
        }
        [HttpPut]
        public async Task<IActionResult> Put(string userId, List<string> roles, int? facilityId)
        {
            // получаем пользователя
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            // обработка ролей
            var userRoles = await _userManager.GetRolesAsync(user);
            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            // обработка факультета
            if (facilityId.HasValue)
            {
                user.FacilityId = facilityId;
                await _userManager.UpdateAsync(user);
            }
            else if (user.FacilityId != null)
            {
                user.FacilityId = null;
                await _userManager.UpdateAsync(user);
            }

            return new JsonResult($"Update successful for user with ID: {user.Id}");
        }
        [HttpGet]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            // Проверяем, существует ли пользователь
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            // Получаем роли пользователя
            var roles = await _userManager.GetRolesAsync(user);

            return new JsonResult(roles);
        }
        [HttpGet]
        public async Task<IActionResult> GetUser(string userId)
        {
            // Проверяем, существует ли пользователь
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            return new JsonResult(user);
        }
    }
}

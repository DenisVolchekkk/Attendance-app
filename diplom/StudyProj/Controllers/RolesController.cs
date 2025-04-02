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
            return new JsonResult(await _userManager.Users.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> RoleList()
        {
            return new JsonResult(await _roleManager.Roles.ToListAsync());
        }
        [HttpPut]
        public async Task<IActionResult> Put(string userId, List<string> roles)
        {
            // получаем пользователя
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            // получем список ролей пользователя
            var userRoles = await _userManager.GetRolesAsync(user);
            // получаем все роли
            var allRoles = _roleManager.Roles.ToList();
            // получаем список ролей, которые были добавлены
            var addedRoles = roles.Except(userRoles);
            // получаем роли, которые были удалены
            var removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

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

using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyProj.Repositories.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
namespace StudyProj.Controllers
{
    [Authorize(Roles = "Deputy Dean,Dean,Chief")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private IGroupService Groups { get; set; }
        UserManager<User> _userManager;

        public GroupController(IGroupService Group, UserManager<User> userManager)
        {
            Groups = Group;
            _userManager = userManager;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()

        {
            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));


            // Если у пользователя нет факультета - возвращаем пустой список
            if (facId.IsNullOrEmpty())
            {
                return new JsonResult(await Groups.GetAllAsync());
            }
            else
            {
                var groups = await Groups.GetAllAsync();
                var filteredGroups = groups.Where(g => g.FacilityId == int.Parse(facId)).ToList();
                return new JsonResult(filteredGroups);
            }

           
        }
        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] Group group)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));


            // Если у пользователя нет факультета - возвращаем пустой список
            if (facId.IsNullOrEmpty())
            {
                return new JsonResult(await Groups.GetAllAsync());
            }
            else
            {
                var groups = await Groups.GetAllAsync();
                var filteredGroups = groups.Where(g => g.FacilityId == int.Parse(facId)).ToList();
                return new JsonResult(filteredGroups);
            }
            
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetGroup(int id)
        {
            var Group = await Groups.GetAsync(id);

            if (Group == null)
            {
                return NotFound();
            }

            return new JsonResult(Group);
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPost]
        public async Task<IActionResult> Post(Group group)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));


            // Если у пользователя нет факультета - возвращаем пустой список
            if (facId.IsNullOrEmpty())
            {
                return BadRequest("Your account is not assigned to any facility");
            }
            group.FacilityId = int.Parse(facId);

            bool success = true;
            try
            {
                var createdGroup = await Groups.CreateAsync(group);
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success
                ? new JsonResult($"Created successfully with ID: {group.Id}")
                : BadRequest("Group creation failed");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPut]
        public async Task<IActionResult> Put(Group Group)
        {
            bool success = true;
            var gr = await Groups.GetAsync(Group.Id);
            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));

            if (facId.IsNullOrEmpty())
            {
                return BadRequest("Your account is not assigned to any facility");
            }
            Group.FacilityId = int.Parse(facId);

            try
            {
                if (gr != null)
                {
                    gr = await Groups.UpdateAsync(Group);
                }
                else
                {
                    success = false;
                }
            }
            catch (Exception)
            {
                success = false;
            }

            return success ? new JsonResult($"Update successful for Group with ID: {gr.Id}") : new JsonResult("Update was not successful");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = true;
            var Group = await Groups.GetAsync(id);

            try
            {
                if (Group != null)
                {
                    await Groups.DeleteAsync(Group.Id);
                }
                else
                {
                    success = false;
                }
            }
            catch (Exception)
            {
                success = false;
            }

            return success ? new JsonResult("Delete successful") : new JsonResult("Delete was not successful");
        }
    }
}

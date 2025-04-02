using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyProj.Repositories.Implementations;
namespace StudyProj.Controllers
{
    [Authorize(Roles = "Deputy Dean,Dean,Chief")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private IGroupService Groups { get; set; }


        public GroupController(IGroupService Group)
        {
            Groups = Group;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()

        {
            return new JsonResult(await Groups.GetAllAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] Group group)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return new JsonResult(await Groups.GetAllAsync(group));
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
        public async Task<IActionResult> Post(Group Group)
        {
            bool success = true;
            Group gr = null;

            try
            {
                gr = await Groups.CreateAsync(Group);
            }
            catch (Exception)
            {
                success = false;
            }

            return success ? new JsonResult($"Created successfully with ID: {gr.Id}") : new JsonResult("Creation failed");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPut]
        public async Task<IActionResult> Put(Group Group)
        {
            bool success = true;
            var gr = await Groups.GetAsync(Group.Id);
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

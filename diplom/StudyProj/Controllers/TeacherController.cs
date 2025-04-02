using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyProj.Repositories.Implementations;
namespace StudyProj.Controllers
{
    [Authorize(Roles = "Deputy Dean,Dean,Chief")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private ITeacherService Teachers { get; set; }


        public TeacherController(ITeacherService Teacher)
        {
            Teachers = Teacher;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()

        { 
            return new JsonResult(await Teachers.GetAllAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] Teacher teacher)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return new JsonResult(await Teachers.GetAllAsync(teacher));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTeacher(int id)
        {
            var Teacher = await Teachers.GetAsync(id);

            if (Teacher == null)
            {
                return NotFound();
            }

            return new JsonResult(Teacher);
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPost]
        public async Task<IActionResult> Post(Teacher Teacher)
        {
            bool success = true;
            Teacher teach = null;

            try
            {
                //Teacher.Facility = await Facilities.GetAsync(Teacher.FacilityId);
                teach = await Teachers.CreateAsync(Teacher);
            }
            catch (Exception)
            {
                success = false;
            }

            return success ? new JsonResult($"Created successfully with ID: {teach.Id}") : new JsonResult("Creation failed");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPut]
        public async Task<IActionResult> Put(Teacher Teacher)
        {
            bool success = true;
            var teach = await Teachers.GetAsync(Teacher.Id);
            try
            {
                if (teach != null)
                {
                    teach = await Teachers.UpdateAsync(Teacher);
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

            return success ? new JsonResult($"Update successful for Teacher with ID: {Teacher.Id}") : new JsonResult("Update was not successful");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = true;
            var teach = await Teachers.GetAsync(id);

            try
            {
                if (teach != null)
                {
                    await Teachers.DeleteAsync(teach.Id);
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

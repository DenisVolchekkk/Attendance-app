using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudyProj.Repositories.Implementations;
using StudyProj.Repositories.Interfaces;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace StudyProj.Controllers
{
    [Authorize(Roles = "Deputy Dean,Dean,Chief,Teacher")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private IAttendanceService Attendances { get; set; }


        public AttendanceController(IAttendanceService Attendance)
        {
            Attendances = Attendance;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));


            // Если у пользователя нет факультета - возвращаем пустой список
            if (facId.IsNullOrEmpty())
            {
                return new JsonResult(await Attendances.GetAllAsync());
            }
            else
            {
                var attendances = await Attendances.GetAllAsync();
                var filteredAttendances = attendances.Where(g => g.Schedule.Group.FacilityId == int.Parse(facId)).ToList();
                return new JsonResult(filteredAttendances);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] Attendance attendance)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var currentUserName = User.FindFirstValue(ClaimTypes.Name);
            var isTeacher = User.IsInRole("Teacher");
            var isChief = User.IsInRole("Chief");
            var isAdmin = User.IsInRole("Dean") || User.IsInRole("Deputy Dean");

            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));

            // Если у пользователя нет факультета - возвращаем пустой список
            if (facId.IsNullOrEmpty())
            {
                return new JsonResult(await Attendances.GetAllAsync(attendance, currentUserName, isChief, isTeacher, isAdmin));
            }
            else
            {
                var attendances = await Attendances.GetAllAsync(attendance, currentUserName, isChief, isTeacher, isAdmin);
                var filteredAttendances = attendances.Where(g => g.Schedule.Group.FacilityId == int.Parse(facId)).ToList();
                return new JsonResult(filteredAttendances);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAttendance(int id)
        {
            var Attendance = await Attendances.GetAsync(id);

            if (Attendance == null)
            {
                return NotFound();
            }

            return new JsonResult(Attendance);
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPost]
        public async Task<IActionResult> Post(Attendance Attendance)
        {
            bool success = true;
            Attendance att = null;

            try
            {
                att = await Attendances.CreateAsync(Attendance);
            }
            catch (Exception)
            {
                success = false;
            }

            return success ? new JsonResult($"Created successfully with ID: {att.Id}") : new JsonResult("Creation failed");
        }
        [HttpPut]
        public async Task<IActionResult> Put(Attendance Attendance)
        {
            bool success = true;
            var att = await Attendances.GetAsync(Attendance.Id);
            try
            {
                if (att != null)
                {
                    att = await Attendances.UpdateAsync(Attendance);
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

            return success ? new JsonResult($"Update successful for Attendance with ID: {att.Id}") : new JsonResult("Update was not successful");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = true;
            var Attendance = await Attendances.GetAsync(id);

            try
            {
                if (Attendance != null)
                {
                    await Attendances.DeleteAsync(Attendance.Id);
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

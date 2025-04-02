using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyProj.Repositories.Implementations;
namespace StudyProj.Controllers
{
    [Authorize(Roles = "Deputy Dean,Dean,Chief")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private IScheduleService Schedules { get; set; }


        public ScheduleController(IScheduleService Schedule)
        {
            Schedules = Schedule;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()

        {
            return new JsonResult(await Schedules.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] Schedule schedule)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return new JsonResult(await Schedules.GetAllAsync(schedule));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetSchedule(int id)
        {
            var Schedule = await Schedules.GetAsync(id);

            if (Schedule == null)
            {
                return NotFound();
            }

            return new JsonResult(Schedule);
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPost]
        public async Task<IActionResult> Post(Schedule Schedule)
        {
            bool success = true;
            Schedule sc = null;

            try
            {
                sc = await Schedules.CreateAsync(Schedule);
            }
            catch (Exception)
            {
                success = false;
            }

            return success ? new JsonResult($"Created successfully with ID: {sc.Id}") : new JsonResult("Creation failed");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPut]
        public async Task<IActionResult> Put(Schedule Schedule)
        {
            bool success = true;
            var sc = await Schedules.GetAsync(Schedule.Id);
            try
            {
                if (sc != null)
                {
                    sc = await Schedules.UpdateAsync(Schedule);
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

            return success ? new JsonResult($"Update successful for Schedule with ID: {sc.Id}") : new JsonResult("Update was not successful");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = true;
            var Schedule = await Schedules.GetAsync(id);

            try
            {
                if (Schedule != null)
                {
                    await Schedules.DeleteAsync(Schedule.Id);
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

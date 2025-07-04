﻿using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudyProj.Repositories.Implementations;
using System.Security.Claims;
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
            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));

            // Если у пользователя нет факультета - возвращаем пустой список
            if (facId.IsNullOrEmpty())
            {
                return new JsonResult(await Schedules.GetAllAsync());
            }
            else
            {
                var schedules = await Schedules.GetAllAsync();
                var filteredSchedules = schedules.Where(g => g.Group.FacilityId == int.Parse(facId)).ToList();
                return new JsonResult(filteredSchedules);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] Schedule schedule)
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
                return new JsonResult(await Schedules.GetAllAsync(schedule, currentUserName, isChief, isTeacher, isAdmin));
            }
            else
            {
                var schedules = await Schedules.GetAllAsync(schedule, currentUserName, isChief, isTeacher, isAdmin);
                var filteredSchedules = schedules.Where(g => g.Group.FacilityId == int.Parse(facId)).ToList();
                return new JsonResult(filteredSchedules);
            }
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
            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));

            try
            {
                if (facId.IsNullOrEmpty())
                {
                    return BadRequest("Your account is not assigned to any facility");
                }
                else
                {
                    sc = await Schedules.CreateAsync(Schedule);
                }
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
            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));

            try
            {
                if (sc != null)
                {
                    if (facId.IsNullOrEmpty())
                    {
                        return BadRequest("Your account is not assigned to any facility");
                    }
                    else
                    {
                        sc = await Schedules.UpdateAsync(Schedule);
                    }
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

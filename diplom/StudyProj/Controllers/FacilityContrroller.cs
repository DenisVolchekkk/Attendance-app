using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using StudyProj.Repositories.Implementations;
using Microsoft.AspNetCore.Authorization;
namespace StudyProj.Controllers
{
    [Authorize(Roles = "Deputy Dean,Dean,Chief")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FacilityController : ControllerBase
    {
        private IFacilityService Facilities { get; set; }

        public FacilityController(IFacilityService facility)
        {
            Facilities = facility;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var facilities = await Facilities.GetAllAsync();
            return new JsonResult(facilities);
        }
        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] Facility fac)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return new JsonResult(await Facilities.GetAllAsync(fac));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetFacility(int id)
        {
            var facility = await Facilities.GetAsync(id);

            if (facility == null)
            {
                return NotFound();
            }

            return new JsonResult(facility);
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPost]
        public async Task<IActionResult> Post(Facility facility)
        {
            bool success = true;
            Facility fac = null;

            try
            {
                fac = await Facilities.CreateAsync(facility);
            }
            catch (Exception)
            {
                success = false;
            }

            return success ? new JsonResult($"Created successfully with ID: {fac.Id}") : new JsonResult("Creation failed");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPut]
        public async Task<IActionResult> Put(Facility facility)
        {
            bool success = true;
            var fac = await Facilities.GetAsync(facility.Id);
            try
            {
                if (fac != null)
                {
                    fac = await Facilities.UpdateAsync(facility);
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

            return success ? new JsonResult($"Update successful for facility with ID: {facility.Id}") : new JsonResult("Update was not successful");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = true;
            var fac = await Facilities.GetAsync(id);

            try
            {
                if (fac != null)
                {
                    await Facilities.DeleteAsync(fac.Id);
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

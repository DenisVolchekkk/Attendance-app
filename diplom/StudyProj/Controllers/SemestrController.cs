using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using StudyProj.Repositories.Interfaces;

namespace StudyProj.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SemestrController : ControllerBase
    {
        private IBaseRepository<Semestr> Semestrs { get; set; }


        public SemestrController(IBaseRepository<Semestr> Semestr)
        {
            Semestrs = Semestr;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()

        {
            return new JsonResult(await Semestrs.GetAllAsync());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetSemestr(int id)
        {
            var Semestr = await Semestrs.GetAsync(id);

            if (Semestr == null)
            {
                return NotFound();
            }

            return new JsonResult(Semestr);
        }
        [HttpPost]
        public async Task<IActionResult> Post(Semestr Semestr)
        {
            bool success = true;
            Semestr sem = null;

            try
            {
                sem = await Semestrs.CreateAsync(Semestr);
            }
            catch (Exception)
            {
                success = false;
            }

            return success ? new JsonResult($"Created successfully with ID: {sem.Id}") : new JsonResult("Creation failed");
        }
        [HttpPut]
        public async Task<IActionResult> Put(Semestr Semestr)
        {
            bool success = true;
            var sem = await Semestrs.GetAsync(Semestr.Id);
            try
            {
                if (sem != null)
                {
                    sem = await Semestrs.UpdateAsync(Semestr);
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

            return success ? new JsonResult($"Update successful for Semestr with ID: {sem.Id}") : new JsonResult("Update was not successful");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = true;
            var Semestr = await Semestrs.GetAsync(id);

            try
            {
                if (Semestr != null)
                {
                    await Semestrs.DeleteAsync(Semestr.Id);
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

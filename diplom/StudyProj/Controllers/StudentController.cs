using Domain.Models;
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
    public class StudentController : ControllerBase
    {
        private IStudentService Students { get; set; }


        public StudentController(IStudentService Student)
        {
            Students = Student;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()

        {
            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));
            if (string.IsNullOrEmpty(facId))
                return Unauthorized("User claim not found");

            // Если у пользователя нет факультета - возвращаем пустой список
            if (facId.IsNullOrEmpty())
            {
                return new JsonResult(await Students.GetAllAsync());
            }
            else
            {
                var students = await Students.GetAllAsync();
                var filteredStudents = students.Where(g => g.Group.FacilityId == int.Parse(facId)).ToList();
                return new JsonResult(filteredStudents);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] Student student)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var facId = (User.FindFirstValue(ClaimTypes.GroupSid));


            // Если у пользователя нет факультета - возвращаем пустой список
            if (facId.IsNullOrEmpty())
            {
                return new JsonResult(await Students.GetAllAsync(student));
            }
            else
            {
                var students = await Students.GetAllAsync(student);
                var filteredStudents = students.Where(g => g.Group.FacilityId == int.Parse(facId)).ToList();
                return new JsonResult(filteredStudents);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetStudent(int id)
        {
            var Student = await Students.GetAsync(id);

            if (Student == null)
            {
                return NotFound();
            }

            return new JsonResult(Student);
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPost]
        public async Task<IActionResult> Post(Student Student)
        {
            bool success = true;
            Student st = null;

            try
            {
                st = await Students.CreateAsync(Student);
            }
            catch (Exception)
            {
                success = false;
            }

            return success ? new JsonResult($"Created successfully with ID: {st.Id}") : new JsonResult("Creation failed");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]
        [HttpPut]
        public async Task<IActionResult> Put(Student Student)
        {
            bool success = true;
            var st = await Students.GetAsync(Student.Id);
            try
            {
                if (st != null)
                {
                    st = await Students.UpdateAsync(Student);
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

            return success ? new JsonResult($"Update successful for Student with ID: {st.Id}") : new JsonResult("Update was not successful");
        }
        [Authorize(Roles = "Deputy Dean,Dean")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = true;
            var Student = await Students.GetAsync(id);

            try
            {
                if (Student != null)
                {
                    await Students.DeleteAsync(Student.Id);
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

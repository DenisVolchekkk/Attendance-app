using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.ViewModel;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class StudentController : Controller
    {
        Uri baseAddress = new Uri("http://ggtuapi.runasp.net/api");
        private readonly HttpClient _client;

        public StudentController()
        {
            // Настроить HttpClientHandler для извлечения токена из куки
            var handler = new HttpClientHandler();

            // Создать HttpClient с обработчиком
            _client = new HttpClient(handler)
            {
                BaseAddress = baseAddress
            };
        }
        private void AddAuthorizationHeader()
        {
            // Получаем токен из куки
            var token = Request.Cookies["AuthToken"]; // Используйте имя куки для токена

            if (!string.IsNullOrEmpty(token))
            {
                // Добавляем токен в заголовок Authorization
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        [HttpGet]
        public IActionResult Index(string sortOrder, string searchStudentName, string searchGroupName, int? pageNumber, int pageSize = 20)
        {
            AddAuthorizationHeader();

            // Параметры сортировки
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StudentNameSortParm"] = sortOrder == "student" ? "student_desc" : "student";
            ViewData["GroupNameSortParm"] = sortOrder == "group" ? "group_desc" : "group";
            ViewData["CurrentPageSize"] = pageSize;

            // Параметры поиска
            ViewData["SearchStudentName"] = searchStudentName;
            ViewData["SearchGroupName"] = searchGroupName;

            IQueryable<Student> StudentList = null;
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Student/Filter?Name={searchStudentName}&Group.Name={searchGroupName}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                StudentList = JsonConvert.DeserializeObject<List<Student>>(data).AsQueryable();
                StudentList = ApplySorting(StudentList, sortOrder);

            }
            else
            {
                int statusCode = (int)response.StatusCode;
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = $"Error code: {statusCode}"
                };
                return View("Error", errorViewModel);
            }

            return View(PaginatedList<Student>.Create(StudentList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        private IQueryable<Student> ApplySorting(IQueryable<Student> StudentList, string sortOrder)
        {
            // Применяем сортировку
            switch (sortOrder)
            {
                case "student":
                    StudentList = StudentList.OrderBy(s => s.Name);
                    return StudentList;
                case "student_desc":
                    StudentList = StudentList.OrderByDescending(s => s.Name);
                    return StudentList;
                case "group":
                    StudentList = StudentList.OrderBy(s => s.Group.Name);
                    return StudentList;
                case "group_desc":
                    StudentList = StudentList.OrderByDescending(s => s.Group.Name);
                    return StudentList;
                default:
                    StudentList = StudentList.OrderBy(s => s.Name);
                    return StudentList;
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            SetViewDataAsync();
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentViewModel model)
        {
            try
            {
                AddAuthorizationHeader();

                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Student/Post", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Student created.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

            return View();
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                AddAuthorizationHeader();

                StudentViewModel Student = new StudentViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Student/GetStudent/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Student = JsonConvert.DeserializeObject<StudentViewModel>(data);
                    SetViewDataAsync(Student.GroupId);
                }
                return View(Student);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(Student Student)
        {
            AddAuthorizationHeader();

            string data = JsonConvert.SerializeObject(Student);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/Student/Put", content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Student updated.";

                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                AddAuthorizationHeader();

                StudentViewModel Student = new StudentViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Student/GetStudent/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Student = JsonConvert.DeserializeObject<StudentViewModel>(data);
                    SetViewDataAsync(Student.GroupId);
                }
                return View(Student);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                AddAuthorizationHeader();

                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/Student/Delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Student deleted.";
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        private void SetViewDataAsync(int? groupId = null)
        {
            AddAuthorizationHeader();
            List<Group> groupList = new List<Group>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Group/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                groupList = JsonConvert.DeserializeObject<List<Group>>(data);
            }
            ViewData["GroupId"] = new SelectList(groupList, "Id", "Name", groupId);
        }
    }
}

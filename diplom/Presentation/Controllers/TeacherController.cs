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
    public class TeacherController : Controller
    {
        Uri baseAddress = new Uri("http://192.168.0.105:5183/api");
        private readonly HttpClient _client;

        public TeacherController()
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
        public IActionResult Index(string SearchTeacherName, int? pageNumber)
        {
            AddAuthorizationHeader();

            ViewData["SearchTeacherName"] = SearchTeacherName;
            //ViewData["SearchFacilityName"] = SearchFacilityName;
            IQueryable<Teacher> TeacherList = null;

            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Teacher/Filter?Name={SearchTeacherName}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                TeacherList = JsonConvert.DeserializeObject<List<Teacher>>(data).AsQueryable();
            }
            else
            {
                // Return an error page with the status code in the ViewModel
                int statusCode = (int)response.StatusCode;
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = $"Error code: {statusCode}"
                };

                return View("Error", errorViewModel);
            }
            int pageSize = 20;

            return View(PaginatedList<Teacher>.Create(TeacherList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult Create()
        {
            //SetViewDataAsync();
            return View();
        }

        [HttpPost]
        public IActionResult Create(TeacherViewModel model)
        {
            try
            {
                AddAuthorizationHeader();

                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Teacher/Post", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Teacher created.";
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

                TeacherViewModel Teacher = new TeacherViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Teacher/GetTeacher/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Teacher = JsonConvert.DeserializeObject<TeacherViewModel>(data);
                }
                //SetViewDataAsync(Teacher.Id);
                return View(Teacher);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(Teacher Teacher)
        {
            AddAuthorizationHeader();

            string data = JsonConvert.SerializeObject(Teacher);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/Teacher/Put", content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Teacher updated.";
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

                TeacherViewModel Teacher = new TeacherViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Teacher/GetTeacher/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Teacher = JsonConvert.DeserializeObject<TeacherViewModel>(data);
                }
                //SetViewDataAsync(Teacher.Id);
                return View(Teacher);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult  DeleteConfirmed(int id)
        {
            try
            {
                AddAuthorizationHeader();

                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/Teacher/Delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Teacher deleted.";
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        //private void SetViewDataAsync(int? facilityId = null)
        //{
        //    AddAuthorizationHeader();
        //    List<Facility> facilityList = new List<Facility>();
        //    HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Facility/GetAll").Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string data = response.Content.ReadAsStringAsync().Result;
        //        facilityList = JsonConvert.DeserializeObject<List<Facility>>(data);
        //    }
        //    ViewData["FacilityId"] = new SelectList(facilityList, "Id", "Name", facilityId);
        //}
    }
}

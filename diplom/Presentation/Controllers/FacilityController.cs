using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Domain.ViewModel;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class FacilityController : Controller
    {
        Uri baseAddress = new Uri("http://192.168.0.105:5183/api");
        private readonly HttpClient _client;

        public FacilityController()
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
        public IActionResult Index(string searchString, int? pageNumber)
        {
            AddAuthorizationHeader();
            ViewData["CurrentFilter"] = searchString;
            IQueryable<Facility> FacilityList = null;

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Facility/Filter?Name=" + searchString).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                FacilityList = JsonConvert.DeserializeObject<List<Facility>>(data).AsQueryable();
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

            return View(PaginatedList<Facility>.Create(FacilityList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(FacilityViewModel model)
        {
            try
            {
                AddAuthorizationHeader();

                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Facility/Post", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Facility created.";
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
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

                FacilityViewModel facility = new FacilityViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Facility/GetFacility/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    facility = JsonConvert.DeserializeObject<FacilityViewModel>(data);
                }
                return View(facility);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(FacilityViewModel facility)
        {
            AddAuthorizationHeader();

            string data = JsonConvert.SerializeObject(facility);
            StringContent content = new StringContent(data,Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/Facility/Put", content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Facility updated.";
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

                FacilityViewModel facility = new FacilityViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Facility/GetFacility/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    facility = JsonConvert.DeserializeObject<FacilityViewModel>(data);
                }
                return View(facility);
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
            AddAuthorizationHeader();

            try
            {
                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/Facility/Delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Facility deleted.";
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }

    }
}

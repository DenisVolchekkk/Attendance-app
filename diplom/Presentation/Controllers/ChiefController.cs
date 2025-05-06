using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Domain.ViewModel;
using Presentation.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Presentation.Controllers
{
    public class ChiefController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5182/api");
        private readonly HttpClient _client;

        public ChiefController()
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
        public IActionResult Index(string sortOrder, string searchString, string searchGroup, int? pageNumber, int pageSize = 20)
        {
            AddAuthorizationHeader();

            // Параметры сортировки
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["GroupSortParm"] = string.IsNullOrEmpty(sortOrder) ? "group_desc" : "group";
            ViewData["CurrentPageSize"] = pageSize;
            ViewData["CurrentFilter"] = searchString;
            ViewData["SearchGroup"] = searchGroup;

            IQueryable<Chief> ChiefList = null;
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/Chief/Filter?Name={searchString}&Group.Name={searchGroup}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ChiefList = JsonConvert.DeserializeObject<List<Chief>>(data).AsQueryable();

                // Применяем сортировку
                switch (sortOrder)
                {
                    case "name_desc":
                        ChiefList = ChiefList.OrderByDescending(c => c.Name);
                        break;
                    case "group_desc":
                        ChiefList = ChiefList.OrderByDescending(c => c.Group.Name);
                        break;
                    case "group":
                        ChiefList = ChiefList.OrderBy(c => c.Group.Name);
                        break;
                    default:
                        ChiefList = ChiefList.OrderBy(c => c.Name);
                        break;
                }
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

            return View(PaginatedList<Chief>.Create(ChiefList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetViewDataAsync();
            return View();
        }

        [HttpPost]
        public IActionResult Create(ChiefViewModel model)
        {
            try
            {
                AddAuthorizationHeader();
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Chief/Post", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Chief created.";
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
                ChiefViewModel Chief = new ChiefViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Chief/GetChief/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Chief = JsonConvert.DeserializeObject<ChiefViewModel>(data);
                    SetViewDataAsync(Chief.GroupId);

                }
                return View(Chief);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(ChiefViewModel Chief)
        {
            AddAuthorizationHeader();
            string data = JsonConvert.SerializeObject(Chief);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/Chief/Put", content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Chief updated.";
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
                ChiefViewModel Chief = new ChiefViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Chief/GetChief/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Chief = JsonConvert.DeserializeObject<ChiefViewModel>(data);
                    SetViewDataAsync(Chief.GroupId);

                }
                return View(Chief);
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
                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/Chief/Delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Chief deleted.";
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        private void SetViewDataAsync( int? groupId = null)
        {
            AddAuthorizationHeader();
            List<Group> groupList = new List<Group>();
            HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Group/GetAll").Result;
            if (response1.IsSuccessStatusCode)
            {
                string data = response1.Content.ReadAsStringAsync().Result;
                groupList = JsonConvert.DeserializeObject<List<Group>>(data);
            }
            ViewData["GroupId"] = new SelectList(groupList, "Id", "Name", groupId);


        }
    }
}

using Domain.Models;
using Domain.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Presentation.Models;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace Presentation.Controllers
{
    public class RolesController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5182/api");
        private readonly HttpClient _client;

        public RolesController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        private void AddAuthorizationHeader()
        {
            var token = Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {

                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        [HttpGet]
        public async Task<IActionResult> UserList(int? pageNumber)
        {
            AddAuthorizationHeader();
            IQueryable<User> users = null;

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Roles/UserList").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<User>>(data).AsQueryable();
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
            var userRoles = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                HttpResponseMessage response2 = _client.GetAsync(_client.BaseAddress + "/Roles/GetUserRoles?userId=" + user.Id).Result;
                IQueryable<string> roles = null;
                if (response.IsSuccessStatusCode)
                {
                    string data = await response2.Content.ReadAsStringAsync();
                    roles = JsonConvert.DeserializeObject<List<string>>(data).AsQueryable();
                }
                else
                {
                    // Return an error page with the status code in the ViewModel
                    int statusCode = (int)response2.StatusCode;
                    var errorViewModel = new ErrorViewModel
                    {
                        RequestId = $"Error code: {statusCode}"
                    };

                    return View("Error", errorViewModel);
                }
               
                userRoles.Add(new UserWithRoleViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FullName = $"{user.LastName} {user.FirstName} {user.FatherName}",
                    Role = roles,
                    Facility = user.Facility?.Name
                });
            }

            int pageSize = 20;
            var paginatedList = PaginatedList<UserWithRoleViewModel>.Create(userRoles.AsQueryable().AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(paginatedList);
        }
        [HttpGet]
        public async Task<IActionResult> RoleList(int? pageNumber)
        {
            AddAuthorizationHeader();
            IQueryable<Role> RoleList = null;

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Roles/RoleList").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                RoleList = JsonConvert.DeserializeObject<List<Role>>(data).AsQueryable();
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

            return View(PaginatedList<Role>.Create(RoleList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id, List<string> Roles, int? facilityId)
        {
            AddAuthorizationHeader();
            try
            {
                HttpResponseMessage rolesResponse = _client.GetAsync(_client.BaseAddress + "/Roles/RoleList").Result;
                HttpResponseMessage facilitiesResponse = _client.GetAsync(_client.BaseAddress + "/Facility/GetAll").Result;

                List<Role> roles = new List<Role>();
                List<Facility> facilities = new List<Facility>();

                if (rolesResponse.IsSuccessStatusCode)
                {
                    string rolesData = await rolesResponse.Content.ReadAsStringAsync();
                    roles = JsonConvert.DeserializeObject<List<Role>>(rolesData);
                }

                if (facilitiesResponse.IsSuccessStatusCode)
                {
                    string facilitiesData = await facilitiesResponse.Content.ReadAsStringAsync();
                    facilities = JsonConvert.DeserializeObject<List<Facility>>(facilitiesData);
                }

                RoleViewModel viewModel = new RoleViewModel
                {
                    Id = id,
                    RoleList = roles,
                    Roles = Roles,
                    SelectedFacilityId = facilityId,
                    Facilities = facilities
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRoles(RoleViewModel model)
        {
            AddAuthorizationHeader();
            bool requireRelogin = false;

            if (model.Roles != null)
            {
                var url = _client.BaseAddress + $"/Roles/Put?userId={model.Id}&facilityId={model.SelectedFacilityId}";

                var jsonContent = JsonConvert.SerializeObject(model.Roles);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Проверяем, меняются ли права текущего пользователя
                    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        requireRelogin = true;

                        TempData["Success"] = "Данные успешно обновлены! Если вы обновили свои права перайдите на аккаунт или дождитесь окончания сессии.";
                }
                else
                {
                    TempData["Error"] = "Ошибка при обновлении данных.";
                }
            }

            if (requireRelogin)
            {
                // Добавляем флаг для отображения специального уведомления
                TempData["RequireRelogin"] = true;
            }

            return RedirectToAction("UserList");
        }

    }
}

using Domain.Models;
using Domain.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Presentation.Models;
using System.Data;
using System.Text;

namespace Presentation.Controllers
{
    public class RolesController : Controller
    {
        Uri baseAddress = new Uri("http://192.168.0.105:5183/api");
        private readonly HttpClient _client;

        public RolesController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        [HttpGet]
        public async Task<IActionResult> UserList(int? pageNumber)
        {
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
                    Role = roles
                });
            }

            int pageSize = 20;
            var paginatedList = PaginatedList<UserWithRoleViewModel>.Create(userRoles.AsQueryable().AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(paginatedList);
        }
        [HttpGet]
        public async Task<IActionResult> RoleList(int? pageNumber)
        {
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
        public async Task<IActionResult> Edit(string id, List<string> Roles)
        {
            try
            { 
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Roles/RoleList").Result;
                List<Role> roles = new List<Role>();

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    roles = JsonConvert.DeserializeObject<List<Role>>(data);
                }
                RoleViewModel viewModel = new RoleViewModel
                {
                    Id = id,
                    RoleList = roles,
                    Roles = Roles
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
        public async Task<IActionResult> UpdateRoles(RoleViewModel newRoles)
        {
            // Обработка обновленных данных
            // Пример сохранения изменений
            if (newRoles.Roles != null)
            {
                var url = _client.BaseAddress + $"/Roles/Put?userId={newRoles.Id}";

                // Create a list of roles

                // Serialize the list to a JSON string
                var jsonContent = JsonConvert.SerializeObject(newRoles.Roles);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync(url, content);
                // Логика сохранения выбранных ролей
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Roles updated successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to update roles.";
                }
            }

            return RedirectToAction("UserList");
        }

    }
}

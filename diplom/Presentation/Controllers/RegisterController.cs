using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
namespace Presentation.Controllers
{
    public class RegisterController : Controller
    {
        Uri baseAddress = new Uri("http://192.168.0.105:5183/api");
        private readonly HttpClient _client;

        public RegisterController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserForRegistrationDto user)
        {
            try
            {
                user.ClientUri = "http://192.168.0.105:5183/api/Accounts/emailconfirmation";
                string data = JsonConvert.SerializeObject(user);

                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Accounts/register", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Проверьте свою почту";
                    return RedirectToAction("Authentication");
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
        public IActionResult Authentication()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authentication(UserForAuthenticationDto user)
        {
            try
            {
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Accounts/authenticate", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<AuthResponseDto>(responseContent);

                    if (result != null && result.IsAuthSuccessful)
                    {
                        // Сохранение токена в куки
                        Response.Cookies.Append("AuthToken", result.Token, new CookieOptions
                        {
                            HttpOnly = true, // Защита от доступа через JavaScript
                            Secure = true,   // Использовать только для HTTPS
                            Expires = DateTimeOffset.UtcNow.AddHours(1) // Время действия токена
                        });

                        TempData["successMessage"] = "Вход успешен";

                        // Разбор токена
                        var claims = ParseJwtToken(result.Token);

                        TempData["userEmail"] = claims["email"];
                        TempData["userRole"] = claims["role"];

                        return RedirectToAction("Index", "Home"); // Перенаправление после успешной аутентификации
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View();
        }
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Append("AuthToken", string.Empty, new CookieOptions
            {
                HttpOnly = true, // Те же параметры для надежности
                Secure = true,   // Использовать только для HTTPS
                Expires = DateTimeOffset.UtcNow.AddDays(-1) // Истекший срок действия
            });
            return RedirectToAction("Index", "Home");
        }
        public IDictionary<string, string> ParseJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Извлечение claims (данных из токена)
            var claims = new Dictionary<string, string>
            {
                { "email", jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value },
                { "role", jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value }
            };

            return claims;
        }

    }
}

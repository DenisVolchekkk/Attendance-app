using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Domain.ViewModel;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class ScheduleController : Controller
    {
        Uri baseAddress = new Uri("http://192.168.0.105:5183/api");
        private readonly HttpClient _client;

        public ScheduleController()
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
        public IActionResult Index(string SearchStartTime, string SearchDayOfWeek, string SearchTeacherName, string SearchDisciplineName, int? pageNumber)
        {
            AddAuthorizationHeader();
            ViewData["SearchStartTime"] = SearchStartTime;
            ViewData["SearchDayOfWeek"] = SearchDayOfWeek;
            ViewData["SearchTeacherName"] = SearchTeacherName;
            ViewData["SearchDisciplineName"] = SearchDisciplineName;
            HttpResponseMessage response;
            TimeSpan.TryParse(SearchStartTime, out var result);
            string formattedTime = result.ToString("hh\\:mm\\:ss");
            string encodedTime = HttpUtility.UrlEncode(formattedTime).ToUpper();
            Enum.TryParse(typeof(DayOfWeek), SearchDayOfWeek, out var res);
            //response = _client.GetAsync($"{_client.BaseAddress}/Schedule/Filter?hour={result.Hour}&minute={result.Minute}&DayOfWeek={SearchDayOfWeek}&Teacher.Name={SearchTeacherName}&Discipline.Name={SearchDisciplineName}").Result;
            if (result.TotalMinutes != 0)
            {
                response = _client.GetAsync($"{_client.BaseAddress}/Schedule/Filter?StartTime={encodedTime}&DayOfWeek={res}&Teacher.Name={SearchTeacherName}&Discipline.Name={SearchDisciplineName}").Result;
            }
            else 
            {
                response = _client.GetAsync($"{_client.BaseAddress}/Schedule/Filter?DayOfWeek={res}&Teacher.Name={SearchTeacherName}&Discipline.Name={SearchDisciplineName}").Result;

            }
            IQueryable<Schedule> ScheduleList = null;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ScheduleList = JsonConvert.DeserializeObject<List<Schedule>>(data).AsQueryable();
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

            return View(PaginatedList<Schedule>.Create(ScheduleList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetViewData();
            return View();
        }

        [HttpPost]
        public IActionResult Create(ScheduleViewModel model)
        {
            try
            {
                AddAuthorizationHeader();

                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Schedule/Post", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Schedule created.";
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

                ScheduleViewModel Schedule = new ScheduleViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Schedule/GetSchedule/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Schedule = JsonConvert.DeserializeObject<ScheduleViewModel>(data);
                    SetViewData(Schedule.DisciplineId, Schedule.TeacherId);
                }
                return View(Schedule);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(ScheduleViewModel Schedule)
        {
            AddAuthorizationHeader();

            string data = JsonConvert.SerializeObject(Schedule);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/Schedule/Put", content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Schedule updated.";
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

                ScheduleViewModel Schedule = new ScheduleViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Schedule/GetSchedule/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Schedule = JsonConvert.DeserializeObject<ScheduleViewModel>(data);
                    SetViewData(Schedule.DisciplineId, Schedule.TeacherId);
                }
                return View(Schedule);
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

                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/Schedule/Delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Schedule deleted.";
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        private void SetViewData(int? disciplineId = null, int ? teacherId = null, int? groupId = null)
        {
            AddAuthorizationHeader();
            List<Discipline> disciplineList = new List<Discipline>();
            HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Discipline/GetAll").Result;
            if (response1.IsSuccessStatusCode)
            {
                string data = response1.Content.ReadAsStringAsync().Result;
                disciplineList = JsonConvert.DeserializeObject<List<Discipline>>(data);
            }

            ViewData["DisciplineId"] = new SelectList(disciplineList, "Id", "Name", disciplineId);

            List<Teacher> teacherList = new List<Teacher>();
            HttpResponseMessage response2 = _client.GetAsync(_client.BaseAddress + "/Teacher/GetAll").Result;
            if (response2.IsSuccessStatusCode)
            {
                string data = response2.Content.ReadAsStringAsync().Result;
                teacherList = JsonConvert.DeserializeObject<List<Teacher>>(data);
            }
            ViewData["TeacherId"] = new SelectList(teacherList, "Id", "Name", teacherId);

            List<Group> groupList = new List<Group>();
            HttpResponseMessage response3 = _client.GetAsync(_client.BaseAddress + "/Group/GetAll").Result;
            if (response3.IsSuccessStatusCode)
            {
                string data = response3.Content.ReadAsStringAsync().Result;
                groupList = JsonConvert.DeserializeObject<List<Group>>(data);
            }
            ViewData["GroupId"] = new SelectList(groupList, "Id", "Name", groupId);
        }
    }
}

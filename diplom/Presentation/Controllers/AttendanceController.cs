using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Domain.ViewModel;
using System;
using System.Web;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class AttendanceController : Controller
    {
        Uri baseAddress = new Uri("http://192.168.0.105:5183/api");
        private readonly HttpClient _client;

        public AttendanceController()
        {
            var handler = new HttpClientHandler();

            _client = new HttpClient(handler)
            {
                BaseAddress = baseAddress
            };
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
        public IActionResult Index(string SearchStudentName, string SearchAttendanceDate, string SearchStartTime, string SearchDisciplineName, int? pageNumber)
        {
            AddAuthorizationHeader();
            ViewData["SearchStudentName"] = SearchStudentName;
            ViewData["SearchAttendanceDate"] = SearchAttendanceDate;
            ViewData["SearchDisciplineName"] = SearchDisciplineName;
            ViewData["SearchStartTime"] = SearchStartTime;
            TimeSpan.TryParse(SearchStartTime, out var result);
            IQueryable<Attendance> AttendanceList = null;
            HttpResponseMessage response;
            string url;
            TimeSpan.TryParse(SearchStartTime, out var res);
            string formattedTime = result.ToString("hh\\:mm\\:ss");
            string encodedTime = HttpUtility.UrlEncode(formattedTime).ToUpper();
            if (!string.IsNullOrEmpty(SearchAttendanceDate))
            {
                    DateTime.TryParse(SearchAttendanceDate, out var dateTime);
                    string formattedDate = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
                    string encodedDate = WebUtility.UrlEncode(formattedDate);
                    url = $"{_client.BaseAddress}/Attendance/Filter?AttendanceDate={encodedDate}&Student.Name={SearchStudentName}&Schedule.Discipline.Name={SearchDisciplineName}";
            }
            else 
            {
                url = $"{_client.BaseAddress}/Attendance/Filter?AttendanceDate={SearchAttendanceDate}&Student.Name={SearchStudentName}&Schedule.Discipline.Name={SearchDisciplineName}";
            }
            if (result.TotalMinutes != 0)
            {
                url+= $"&Schedule.StartTime={encodedTime}";
            }

            response = _client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                AttendanceList = JsonConvert.DeserializeObject<List<Attendance>>(data).AsQueryable();
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

            return View(PaginatedList<Attendance>.Create(AttendanceList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        [HttpGet]
        public IActionResult Create()
        {
            SetViewData();
            return View();
        }

        [HttpPost]
        public IActionResult Create(AttendanceViewModel model)
        {
            try
            {
                AddAuthorizationHeader();
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Attendance/Post", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Attendance created.";
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
                AttendanceViewModel Attendance = new AttendanceViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Attendance/GetAttendance/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Attendance = JsonConvert.DeserializeObject<AttendanceViewModel>(data);
                    SetViewData(Attendance.StudentId, Attendance.ScheduleId);
                }
                return View(Attendance);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(AttendanceViewModel Attendance)
        {
            AddAuthorizationHeader();
            string data = JsonConvert.SerializeObject(Attendance);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/Attendance/Put", content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Attendance updated.";
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
                AttendanceViewModel Attendance = new AttendanceViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Attendance/GetAttendance/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Attendance = JsonConvert.DeserializeObject<AttendanceViewModel>(data);
                    SetViewData(Attendance.StudentId, Attendance.ScheduleId);
                }
                return View(Attendance);
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
                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/Attendance/Delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Attendance deleted.";
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        [HttpPost]
        public async Task<IActionResult> TogglePresence(int id)
        {
            AddAuthorizationHeader();
            AttendanceViewModel Attendance = new AttendanceViewModel();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Attendance/GetAttendance/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                Attendance = JsonConvert.DeserializeObject<AttendanceViewModel>(data);
            }
            if (Attendance == null)
            {
                return Json(new { success = false });
            }

            Attendance.IsPresent = !Attendance.IsPresent; // Переключаем статус
            string data2 = JsonConvert.SerializeObject(Attendance);
            StringContent content = new StringContent(data2, Encoding.UTF8, "application/json");
            HttpResponseMessage response2 = _client.PutAsync(_client.BaseAddress + "/Attendance/Put", content).Result;

            return Json(new { success = true, isPresent = Attendance.IsPresent });
        }
        private void SetViewData(int? studentId = null, int? scheduleId = null)
        {
            AddAuthorizationHeader();
            List<Student> studentList = new List<Student>();
            HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Student/GetAll").Result;
            if (response1.IsSuccessStatusCode)
            {
                string data = response1.Content.ReadAsStringAsync().Result;
                studentList = JsonConvert.DeserializeObject<List<Student>>(data);
            }
            ViewData["StudentId"] = new SelectList(studentList, "Id", "Name", studentId);

            List<Schedule> scheduleList = new List<Schedule>();
            HttpResponseMessage response2 = _client.GetAsync(_client.BaseAddress + "/Schedule/GetAll").Result;
            if (response2.IsSuccessStatusCode)
            {
                string data = response2.Content.ReadAsStringAsync().Result;
                scheduleList = JsonConvert.DeserializeObject<List<Schedule>>(data);
            }

            ViewBag.ScheduleItems = new SelectList(
                scheduleList.Select(s => new
                {
                    Id = s.Id,
                    Text = $"{s.Discipline.Name} - {s.DayOfWeek} - {s.StartTime:hh\\:mm}",
                    DataAttributes = new { DayOfWeek = (int)s.DayOfWeek }
                }),
                "Id",
                "Text",
                scheduleId
            );
            ViewBag.ScheduleDayOfWeek = scheduleList.ToDictionary(s => s.Id, s => (int)s.DayOfWeek);

        }

    }

}

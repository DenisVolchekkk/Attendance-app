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
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Linq;

namespace Presentation.Controllers
{
    public class AttendanceController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5182/api");
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
        public IActionResult Index(string sortOrder, string SearchStudentName, string SearchAttendanceDate, string SearchStartTime,
            string SearchDisciplineName, string SearchTeacherName, string SearchChiefName, string SearchGroupName, int? pageNumber, int pageSize = 20)
        {
            AddAuthorizationHeader();

            // Сохраняем параметры сортировки в ViewData
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StudentSortParm"] = string.IsNullOrEmpty(sortOrder) ? "student_desc" : "";
            ViewData["TimeSortParm"] = sortOrder == "time" ? "time_desc" : "time";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["DisciplineSortParm"] = sortOrder == "discipline" ? "discipline_desc" : "discipline";
            ViewData["PresenceSortParm"] = sortOrder == "presence" ? "presence_desc" : "presence";
            ViewData["AuditorySortParm"] = sortOrder == "auditory" ? "auditory_desc" : "auditory";
            ViewData["GroupSortParm"] = sortOrder == "group" ? "group_desc" : "group";
            ViewData["CurrentPageSize"] = pageSize;
            // Сохраняем параметры поиска
            ViewData["SearchStudentName"] = SearchStudentName;
            ViewData["SearchAttendanceDate"] = SearchAttendanceDate;
            ViewData["SearchDisciplineName"] = SearchDisciplineName;
            ViewData["SearchStartTime"] = SearchStartTime;
            ViewData["SearchTeacherName"] = SearchTeacherName;
            ViewData["SearchChiefName"] = SearchChiefName;
            ViewData["SearchGroupName"] = SearchGroupName;

            TimeSpan.TryParse(SearchStartTime, out var timeResult);
            DateTime.TryParse(SearchAttendanceDate, out var dateTime);

            string url = $"{_client.BaseAddress}/Attendance/Filter?" +
                $"AttendanceDate={(string.IsNullOrEmpty(SearchAttendanceDate) ?
                    WebUtility.UrlEncode(SearchAttendanceDate) :
                    WebUtility.UrlEncode(dateTime.ToString("yyyy-MM-ddTHH:mm:ss")))}" +
                $"&Student.Name={SearchStudentName}" +
                $"&Schedule.Discipline.Name={SearchDisciplineName}" +
                $"&Schedule.Teacher.Name={SearchTeacherName}" +
                $"&Schedule.Group.Chief.Name={SearchChiefName}" +
                $"&Schedule.Group.Name={SearchGroupName}" +
                (timeResult.TotalMinutes != 0 ? $"&Schedule.StartTime={HttpUtility.UrlEncode(timeResult.ToString("hh\\:mm\\:ss")).ToUpper()}" : "");

            IQueryable<Attendance> AttendanceList = null;
            HttpResponseMessage response;

            response = _client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                AttendanceList = JsonConvert.DeserializeObject<List<Attendance>>(data).AsQueryable();
                AttendanceList = ApplySorting(AttendanceList, sortOrder);
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
            var russianDayNames = new Dictionary<DayOfWeek, string>
{
    { DayOfWeek.Sunday, "Воскресенье" },
    { DayOfWeek.Monday, "Понедельник" },
    { DayOfWeek.Tuesday, "Вторник" },
    { DayOfWeek.Wednesday, "Среда" },
    { DayOfWeek.Thursday, "Четверг" },
    { DayOfWeek.Friday, "Пятница" },
    { DayOfWeek.Saturday, "Суббота" }
};

            ViewBag.ScheduleItems = new SelectList(
                scheduleList.Select(s => new
                {
                    Id = s.Id,
                    Text = $"{s.Discipline.Name} - {(s.DayOfWeek.HasValue ? russianDayNames[s.DayOfWeek.Value] : "День не указан")} - {s.StartTime:hh\\:mm}",
                    DataAttributes = new { DayOfWeek = s.DayOfWeek.HasValue ? (int)s.DayOfWeek.Value : -1 }
                }),
                "Id",
                "Text",
                scheduleId
            );

            ViewBag.ScheduleDayOfWeek = scheduleList.ToDictionary(
    s => s.Id,
    s => s.DayOfWeek.HasValue ? (int)s.DayOfWeek.Value : -1
);

        }
        [HttpPost]
        public IActionResult GenerateReport(string sortOrder, string SearchStudentName, string SearchAttendanceDate,
    string SearchStartTime, string SearchDisciplineName, string SearchTeacherName,
    string SearchChiefName, string SearchGroupName)
        {
            AddAuthorizationHeader();

            // Получаем данные для отчета
            DateTime.TryParse(SearchAttendanceDate, out var dateTime);
            TimeSpan.TryParse(SearchStartTime, out var timeResult);

            string url = $"{_client.BaseAddress}/Attendance/Filter?" +
                $"AttendanceDate={(string.IsNullOrEmpty(SearchAttendanceDate) ?
                    WebUtility.UrlEncode(SearchAttendanceDate) :
                    WebUtility.UrlEncode(dateTime.ToString("yyyy-MM-ddTHH:mm:ss")))}" +
                $"&Student.Name={SearchStudentName}" +
                $"&Schedule.Discipline.Name={SearchDisciplineName}" +
                $"&Schedule.Teacher.Name={SearchTeacherName}" +
                $"&Schedule.Group.Chief.Name={SearchChiefName}" +
                $"&Schedule.Group.Name={SearchGroupName}" +
                (timeResult.TotalMinutes != 0 ? $"&Schedule.StartTime={HttpUtility.UrlEncode(timeResult.ToString("hh\\:mm\\:ss")).ToUpper()}" : "");

            HttpResponseMessage response = _client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                TempData["errorMessage"] = "Ошибка при получении данных для отчета";
                return RedirectToAction("Index");
            }

            string data = response.Content.ReadAsStringAsync().Result;
            var attendances = JsonConvert.DeserializeObject<List<Attendance>>(data);

            // Группируем данные по группе и дисциплине
            var groupedData = attendances
                .GroupBy(a => new {
                    GroupName = a.Schedule?.Group?.Name ?? "Без группы",
                    DisciplineName = a.Schedule?.Discipline?.Name ?? "Без дисциплины",
                    TeacherName = a.Schedule?.Teacher?.Name ?? "Без преподавателя"
                })
                .OrderBy(g => g.Key.GroupName)
                .ThenBy(g => g.Key.DisciplineName);

            // Создаем книгу Excel
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Посещаемость");

            // Стили для форматирования
            ICellStyle groupHeaderStyle = CreateHeaderStyle(workbook, IndexedColors.LightBlue.Index);
            ICellStyle disciplineHeaderStyle = CreateHeaderStyle(workbook, IndexedColors.LightYellow.Index);
            ICellStyle teacherHeaderStyle = CreateHeaderStyle(workbook, IndexedColors.Grey25Percent.Index);
            ICellStyle dateHeaderStyle = CreateHeaderStyle(workbook, IndexedColors.Grey25Percent.Index);
            ICellStyle centeredStyle = CreateCenteredStyle(workbook);

            int rowNum = 0;

            foreach (var group in groupedData)
            {
                // Заголовок группы (объединение 6 столбцов)
                IRow groupRow = sheet.CreateRow(rowNum++);
                groupRow.CreateCell(0).SetCellValue(group.Key.GroupName);
                sheet.AddMergedRegion(new CellRangeAddress(groupRow.RowNum, groupRow.RowNum, 0, 5));
                groupRow.GetCell(0).CellStyle = groupHeaderStyle;

                // Заголовок дисциплины (объединение 6 столбцов)
                IRow disciplineRow = sheet.CreateRow(rowNum++);
                disciplineRow.CreateCell(0).SetCellValue(group.Key.DisciplineName);
                sheet.AddMergedRegion(new CellRangeAddress(disciplineRow.RowNum, disciplineRow.RowNum, 0, 5));
                disciplineRow.GetCell(0).CellStyle = disciplineHeaderStyle;

                IRow teacherRow = sheet.CreateRow(rowNum++);
                teacherRow.CreateCell(0).SetCellValue(group.Key.TeacherName);
                sheet.AddMergedRegion(new CellRangeAddress(teacherRow.RowNum, teacherRow.RowNum, 0, 5));
                teacherRow.GetCell(0).CellStyle = teacherHeaderStyle;
                // Получаем уникальные даты для столбцов
                var dates = group
                    .Where(a => a.AttendanceDate.HasValue)
                    .Select(a => a.AttendanceDate.Value.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                // Заголовки дат
                IRow dateHeaderRow = sheet.CreateRow(rowNum++);
                dateHeaderRow.CreateCell(0).SetCellValue("ФИО студента");

                for (int i = 0; i < dates.Count; i++)
                {
                    dateHeaderRow.CreateCell(i + 1).SetCellValue(dates[i].ToString("dd.MM.yyyy"));
                    dateHeaderRow.GetCell(i + 1).CellStyle = dateHeaderStyle;
                }

                // Группируем студентов
                var students = group
                    .GroupBy(a => a.Student?.Name ?? "Неизвестный студент")
                    .OrderBy(g => g.Key);

                // Данные по студентам
                foreach (var student in students)
                {
                    IRow studentRow = sheet.CreateRow(rowNum++);
                    studentRow.CreateCell(0).SetCellValue(student.Key);

                    // Заполняем посещения для каждой даты
                    for (int i = 0; i < dates.Count; i++)
                    {
                        var attendance = student.FirstOrDefault(a =>
                            a.AttendanceDate.HasValue &&
                            a.AttendanceDate.Value.Date == dates[i]);

                        var cell = studentRow.CreateCell(i + 1);
                        cell.SetCellValue(attendance?.IsPresent == true ? "" : "н");
                        cell.CellStyle = centeredStyle;
                    }
                }

                // Пустая строка между группами
                rowNum++;
            }

            // Автонастройка ширины столбцов
            for (int i = 0; i < 6; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            // Возвращаем файл
            byte[] fileContents;
            using (var tempStream = new MemoryStream())
            {
                workbook.Write(tempStream);
                fileContents = tempStream.ToArray();
            }

            return File(fileContents,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Посещаемость_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        private ICellStyle CreateHeaderStyle(IWorkbook workbook, short backgroundColor)
        {
            ICellStyle style = workbook.CreateCellStyle();
            IFont font = workbook.CreateFont();
            font.IsBold = true;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.FillForegroundColor = backgroundColor;
            style.FillPattern = FillPattern.SolidForeground;
            return style;
        }

        private ICellStyle CreateCenteredStyle(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            return style;
        }
        private IQueryable<Attendance> ApplySorting(IQueryable<Attendance> attendanceList, string sortOrder)
        {
            switch (sortOrder)
            {
                case "student_desc":
                    return attendanceList.OrderByDescending(a => a.Student.Name);
                case "time":
                    return attendanceList.OrderBy(a => a.Schedule.StartTime);
                case "time_desc":
                    return attendanceList.OrderByDescending(a => a.Schedule.StartTime);
                case "date":
                    return attendanceList.OrderBy(a => a.AttendanceDate);
                case "date_desc":
                    return attendanceList.OrderByDescending(a => a.AttendanceDate);
                case "discipline":
                    return attendanceList.OrderBy(a => a.Schedule.Discipline.Name);
                case "discipline_desc":
                    return attendanceList.OrderByDescending(a => a.Schedule.Discipline.Name);
                case "presence":
                    return attendanceList.OrderBy(a => a.IsPresent);
                case "presence_desc":
                    return attendanceList.OrderByDescending(a => a.IsPresent);
                case "auditory":
                    return attendanceList.OrderBy(a => a.Schedule.Auditory);
                case "auditory_desc":
                    return attendanceList.OrderByDescending(a => a.Schedule.Auditory);
                case "group":
                    return attendanceList.OrderBy(a => a.Schedule.Group.Name);
                case "group_desc":
                    return attendanceList.OrderByDescending(a => a.Schedule.Group.Name);
                default:
                    return attendanceList.OrderBy(a => a.Student.Name);
            }
        }
    }
    

}

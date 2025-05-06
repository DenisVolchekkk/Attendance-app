using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Domain.ViewModel;
using Presentation.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;

namespace Presentation.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly Dictionary<string, DayOfWeek> _russianToDayOfWeek = new Dictionary<string, DayOfWeek>
            {
                {"понедельник", DayOfWeek.Monday},
                {"вторник", DayOfWeek.Tuesday},
                {"среда", DayOfWeek.Wednesday},
                {"четверг", DayOfWeek.Thursday},
                {"пятница", DayOfWeek.Friday},
                {"суббота", DayOfWeek.Saturday},
                {"воскресенье", DayOfWeek.Sunday}
            };
        Uri baseAddress = new Uri("http://localhost:5182/api");
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
        public IActionResult Index(string sortOrder, string SearchStartTime, string SearchDayOfWeek,
    string SearchTeacherName, string SearchDisciplineName, string SearchSemestr, string SearchStudyYear, int? pageNumber, int pageSize = 20)
        {
            AddAuthorizationHeader();

            // Параметры сортировки
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StartTimeSortParm"] = sortOrder == "startTime" ? "startTime_desc" : "startTime";
            ViewData["EndTimeSortParm"] = sortOrder == "endTime" ? "endTime_desc" : "endTime";
            ViewData["DayOfWeekSortParm"] = sortOrder == "dayOfWeek" ? "dayOfWeek_desc" : "dayOfWeek";
            ViewData["GroupSortParm"] = sortOrder == "group" ? "group_desc" : "group";
            ViewData["TeacherSortParm"] = sortOrder == "teacher" ? "teacher_desc" : "teacher";
            ViewData["DisciplineSortParm"] = sortOrder == "discipline" ? "discipline_desc" : "discipline";
            ViewData["AuditorySortParm"] = sortOrder == "auditory" ? "auditory_desc" : "auditory";
            ViewData["SemestrSortParm"] = sortOrder == "semestr" ? "semestr_desc" : "semestr";
            ViewData["StudyYearSortParm"] = sortOrder == "studyYear" ? "studyYear_desc" : "studyYear";
            ViewData["CurrentPageSize"] = pageSize;

            // Параметры поиска
            ViewData["SearchStartTime"] = SearchStartTime;
            ViewData["SearchDayOfWeek"] = SearchDayOfWeek;
            ViewData["SearchTeacherName"] = SearchTeacherName;
            ViewData["SearchDisciplineName"] = SearchDisciplineName;
            ViewData["SearchSemestr"] = SearchSemestr;
            ViewData["SearchStudyYear"] = SearchStudyYear;
            HttpResponseMessage response;
            TimeSpan.TryParse(SearchStartTime, out var result);
            string formattedTime = result.ToString("hh\\:mm\\:ss");
            string encodedTime = HttpUtility.UrlEncode(formattedTime).ToUpper();

            // Преобразование русского названия дня недели в enum
            object res = null;
            if (!string.IsNullOrEmpty(SearchDayOfWeek))
            {
                var lowerSearch = SearchDayOfWeek.ToLower();
                if (_russianToDayOfWeek.ContainsKey(lowerSearch))
                {
                    res = _russianToDayOfWeek[lowerSearch];
                }
                else
                {
                    // Пробуем распарсить английское название (на случай, если ввели по-английски)
                    Enum.TryParse(typeof(DayOfWeek), SearchDayOfWeek, true, out res);
                }
            }

            if (result.TotalMinutes != 0)
            {
                response = _client.GetAsync($"{_client.BaseAddress}/Schedule/Filter?StartTime={encodedTime}&DayOfWeek={res}&Teacher.Name={SearchTeacherName}&Discipline.Name={SearchDisciplineName}&Semestr={SearchSemestr}&StudyYear={SearchStudyYear}").Result;
            }
            else
            {
                response = _client.GetAsync($"{_client.BaseAddress}/Schedule/Filter?DayOfWeek={res}&Teacher.Name={SearchTeacherName}&Discipline.Name={SearchDisciplineName}&Semestr={SearchSemestr}&StudyYear={SearchStudyYear}").Result;
            }

            IQueryable<Schedule> ScheduleList = null;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ScheduleList = JsonConvert.DeserializeObject<List<Schedule>>(data).AsQueryable();
                ScheduleList = ApplySorting(ScheduleList, sortOrder);
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

            return View(PaginatedList<Schedule>.Create(ScheduleList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        private IQueryable<Schedule> ApplySorting(IQueryable<Schedule> scheduleList, string sortOrder)
        {
            switch (sortOrder)
            {
                case "startTime":
                    return scheduleList.OrderBy(s => s.StartTime);
                case "startTime_desc":
                    return scheduleList.OrderByDescending(s => s.StartTime);
                case "endTime":
                    return scheduleList.OrderBy(s => s.EndTime);
                case "endTime_desc":
                    return scheduleList.OrderByDescending(s => s.EndTime);
                case "dayOfWeek":
                    return scheduleList.OrderBy(s => s.DayOfWeek);
                case "dayOfWeek_desc":
                    return scheduleList.OrderByDescending(s => s.DayOfWeek);
                case "group":
                    return scheduleList.OrderBy(s => s.Group.Name);
                case "group_desc":
                    return scheduleList.OrderByDescending(s => s.Group.Name);
                case "teacher":
                    return scheduleList.OrderBy(s => s.Teacher.Name);
                case "teacher_desc":
                    return scheduleList.OrderByDescending(s => s.Teacher.Name);
                case "discipline":
                    return scheduleList.OrderBy(s => s.Discipline.Name);
                case "discipline_desc":
                    return scheduleList.OrderByDescending(s => s.Discipline.Name);
                case "auditory":
                    return scheduleList.OrderBy(s => s.Auditory);
                case "auditory_desc":
                    return scheduleList.OrderByDescending(s => s.Auditory);
                case "semestr":
                    return scheduleList.OrderBy(s => s.Semestr);
                case "semestr_desc":
                    return scheduleList.OrderByDescending(s => s.Semestr);
                case "studyYear":
                    return scheduleList.OrderBy(s => s.StudyYear);
                case "studyYear_desc":
                    return scheduleList.OrderByDescending(s => s.StudyYear);
                default:
                    return scheduleList.OrderBy(s => s.StartTime);
            }
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
                {
                    string errorContent = response.Content.ReadAsStringAsync().Result;

                    TempData["errorMessage"] = $"Error: {response.StatusCode} - {errorContent}";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

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
            else
            {
                string errorContent = response.Content.ReadAsStringAsync().Result;

                TempData["errorMessage"] = $"Error: {response.StatusCode} - {errorContent}";
                return View();
            }
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
                    return RedirectToAction("Index");

                }
                else
                {
                    string errorContent = response.Content.ReadAsStringAsync().Result;

                    TempData["errorMessage"] = $"Error: {response.StatusCode} - {errorContent}";
                    return View();
                }

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
        [HttpGet]
        public IActionResult TeacherSchedule(int teacherId)
        {
            AddAuthorizationHeader();

            // Получаем имя преподавателя
            var teacherResponse = _client.GetAsync($"{_client.BaseAddress}/Teacher/GetTeacher/{teacherId}").Result;
            if (!teacherResponse.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel { RequestId = "Преподаватель не найден" });
            }
            var teacher = JsonConvert.DeserializeObject<Teacher>(teacherResponse.Content.ReadAsStringAsync().Result);
            ViewData["TeacherName"] = teacher.Name;

            // Получаем расписание преподавателя
            var scheduleResponse = _client.GetAsync($"{_client.BaseAddress}/Schedule/Filter?Teacher.Name={teacher.Name}").Result;
            if (!scheduleResponse.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel { RequestId = "Ошибка при получении расписания" });
            }

            var schedules = JsonConvert.DeserializeObject<List<Schedule>>(scheduleResponse.Content.ReadAsStringAsync().Result);

            // Группируем по дням недели и фильтруем null значения
            var groupedSchedules = schedules
                .Where(s => s.DayOfWeek.HasValue)
                .GroupBy(s => s.DayOfWeek.Value)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.StartTime).ToList());

            return View(groupedSchedules);
        }
        [HttpPost]
        public IActionResult GenerateReport(string sortOrder, string SearchStartTime, string SearchDayOfWeek,
            string SearchTeacherName, string SearchDisciplineName)
        {
            AddAuthorizationHeader();

            // Получаем данные для отчета
            object res = null;
            if (!string.IsNullOrEmpty(SearchDayOfWeek))
            {
                var lowerSearch = SearchDayOfWeek.ToLower();
                if (_russianToDayOfWeek.ContainsKey(lowerSearch))
                {
                    res = _russianToDayOfWeek[lowerSearch];
                }
                else
                {
                    Enum.TryParse(typeof(DayOfWeek), SearchDayOfWeek, true, out res);
                }
            }

            HttpResponseMessage response;
            if (TimeSpan.TryParse(SearchStartTime, out var result) && result.TotalMinutes != 0)
            {
                string formattedTime = result.ToString("hh\\:mm\\:ss");
                string encodedTime = HttpUtility.UrlEncode(formattedTime).ToUpper();
                response = _client.GetAsync($"{_client.BaseAddress}/Schedule/Filter?StartTime={encodedTime}&DayOfWeek={res}&Teacher.Name={SearchTeacherName}&Discipline.Name={SearchDisciplineName}").Result;
            }
            else
            {
                response = _client.GetAsync($"{_client.BaseAddress}/Schedule/Filter?DayOfWeek={res}&Teacher.Name={SearchTeacherName}&Discipline.Name={SearchDisciplineName}").Result;
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["errorMessage"] = "Ошибка при получении данных для отчета";
                return RedirectToAction("Index");
            }

            string data = response.Content.ReadAsStringAsync().Result;
            var schedules = JsonConvert.DeserializeObject<List<Schedule>>(data);
            schedules = ApplySorting(schedules.AsQueryable(), sortOrder).ToList();

            // Создаем книгу Excel
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Расписание");

            // Стили для форматирования
            ICellStyle headerStyle = workbook.CreateCellStyle();
            IFont headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);
            headerStyle.Alignment = HorizontalAlignment.Center;

            ICellStyle dayHeaderStyle = workbook.CreateCellStyle();
            dayHeaderStyle.CloneStyleFrom(headerStyle);
            dayHeaderStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            dayHeaderStyle.FillPattern = FillPattern.SolidForeground;

            // Заголовки столбцов
            IRow columnHeaderRow = sheet.CreateRow(0);
            columnHeaderRow.CreateCell(0).SetCellValue("Время начала");
            columnHeaderRow.CreateCell(1).SetCellValue("Время окончания");
            columnHeaderRow.CreateCell(2).SetCellValue("Дисциплина");
            columnHeaderRow.CreateCell(3).SetCellValue("Преподаватель");
            columnHeaderRow.CreateCell(4).SetCellValue("Аудитория");
            // Применяем стиль к заголовкам столбцов
            for (int i = 0; i < 5; i++)
            {
                columnHeaderRow.GetCell(i).CellStyle = headerStyle;
            }

            // Группируем расписание по группам, затем по дням недели
            var groupSchedules = schedules
                .GroupBy(s => s.Group.Name)
                .OrderBy(g => g.Key);

            int currentRow = 1;
            foreach (var group in groupSchedules)
            {
                // Название группы (объединение 5 столбцов)
                IRow groupRow = sheet.CreateRow(currentRow++);
                groupRow.CreateCell(0).SetCellValue(group.Key);
                sheet.AddMergedRegion(new CellRangeAddress(groupRow.RowNum, groupRow.RowNum, 0, 4));
                groupRow.GetCell(0).CellStyle = headerStyle;

                // Группируем занятия по дням недели
                var dayGroups = group
                    .GroupBy(g => g.DayOfWeek)
                    .OrderBy(g => g.Key);

                foreach (var dayGroup in dayGroups)
                {
                    // Название дня недели (объединение 5 столбцов)
                    IRow dayRow = sheet.CreateRow(currentRow++);
                    dayRow.CreateCell(0).SetCellValue(
                        _russianToDayOfWeek.FirstOrDefault(x => x.Value == dayGroup.Key).Key);
                    sheet.AddMergedRegion(new CellRangeAddress(dayRow.RowNum, dayRow.RowNum, 0, 4));
                    dayRow.GetCell(0).CellStyle = dayHeaderStyle;

                    // Занятия в этот день
                    foreach (var schedule in dayGroup.OrderBy(s => s.StartTime))
                    {
                        IRow lessonRow = sheet.CreateRow(currentRow++);
                        lessonRow.CreateCell(0).SetCellValue(schedule.StartTime.ToString(@"hh\:mm"));
                        lessonRow.CreateCell(1).SetCellValue(schedule.EndTime.ToString(@"hh\:mm"));
                        lessonRow.CreateCell(2).SetCellValue(schedule.Discipline.Name);
                        lessonRow.CreateCell(3).SetCellValue(schedule.Teacher.Name);
                        lessonRow.CreateCell(4).SetCellValue(schedule.Auditory);
                    }


                }
                // Пустая строка после дня
                currentRow++;
            }

            // Автонастройка ширины столбцов
            for (int i = 0; i < 5; i++)
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
                $"Расписание_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
    }
}

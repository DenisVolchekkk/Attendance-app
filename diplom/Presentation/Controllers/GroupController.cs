using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.ViewModel;
using Presentation.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Presentation.Controllers
{
    public class GroupController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5182/api");
        private readonly HttpClient _client;

        public GroupController()
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
        public IActionResult Index(string sortOrder, string searchGroupName,
            string searchFacilityName, int? pageNumber, int pageSize = 20)
        {
            AddAuthorizationHeader();

            // Параметры сортировки
            ViewData["CurrentSort"] = sortOrder;
            ViewData["GroupNameSortParm"] = sortOrder == "group" ? "group_desc" : "group";
            ViewData["ChiefNameSortParm"] = sortOrder == "chief" ? "chief_desc" : "chief";
            ViewData["FacilityNameSortParm"] = sortOrder == "facility" ? "facility_desc" : "facility";
            ViewData["CurrentPageSize"] = pageSize;

            // Параметры поиска
            ViewData["SearchGroupName"] = searchGroupName;
            //ViewData["SearchChiefName"] = searchChiefName;
            ViewData["SearchFacilityName"] = searchFacilityName;

            IQueryable<Group> GroupList = null;
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Group/Filter?Name={searchGroupName}&Facility.Name={searchFacilityName}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                GroupList = JsonConvert.DeserializeObject<List<Group>>(data).AsQueryable();
                GroupList = ApplySorting(GroupList, sortOrder);
                // Применяем сортировку

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

            return View(PaginatedList<Group>.Create(GroupList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        private IQueryable<Group> ApplySorting(IQueryable<Group> GroupList, string sortOrder)
        {
            switch (sortOrder)
            {
                case "group":
                    GroupList = GroupList.OrderBy(g => g.Name);
                    return GroupList;
                case "group_desc":
                    GroupList = GroupList.OrderByDescending(g => g.Name);
                    return GroupList;
                //case "chief":
                //    GroupList = GroupList.OrderBy(g => g.Chief != null ? g.Chief.Name ?? string.Empty : string.Empty);
                //    return GroupList;
                //case "chief_desc":
                //    GroupList = GroupList.OrderByDescending(g => g.Chief != null ? g.Chief.Name ?? string.Empty : string.Empty);
                //    return GroupList;
                case "facility":
                    GroupList = GroupList.OrderBy(g => g.Facility != null ? g.Facility.Name ?? string.Empty : string.Empty);
                    return GroupList;
                case "facility_desc":
                    GroupList = GroupList.OrderByDescending(g => g.Facility != null ? g.Facility.Name ?? string.Empty : string.Empty);
                    return GroupList;
                default:
                    GroupList = GroupList.OrderBy(g => g.Name);
                    return GroupList;
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            SetViewDataAsync();
            return View();
        }

        [HttpPost]
        public IActionResult Create(GroupViewModel model)
        {
            try
            {
                AddAuthorizationHeader();
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Group/Post", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Group created.";
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
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                AddAuthorizationHeader();
                GroupViewModel Group = new GroupViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Group/GetGroup/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Group =  JsonConvert.DeserializeObject<GroupViewModel>(data);
                    SetViewDataAsync(Group.FacilityId);
                }
                return View(Group);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(GroupViewModel Group)
        {
            AddAuthorizationHeader();

            string data = JsonConvert.SerializeObject(Group);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/Group/Put", content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Group updated.";
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

                GroupViewModel Group = new GroupViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Group/GetGroup/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Group = JsonConvert.DeserializeObject<GroupViewModel>(data);
                    SetViewDataAsync(Group.FacilityId);
                }
                return View(Group);
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

                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/Group/Delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Group deleted.";
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
        private void SetViewDataAsync(int? facilityId = null, int? chiefId = null)
        {
            AddAuthorizationHeader();
            List<Facility> facilityList = new List<Facility>();
            HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Facility/GetAll").Result;
            if (response1.IsSuccessStatusCode)
            {
                string data = response1.Content.ReadAsStringAsync().Result;
                facilityList = JsonConvert.DeserializeObject<List<Facility>>(data);
            }
            ViewData["FacilityId"] = new SelectList(facilityList, "Id", "Name", facilityId);

            List<Chief> chiefList = new List<Chief>();
            HttpResponseMessage response2 = _client.GetAsync(_client.BaseAddress + "/Chief/GetAll").Result;
            if (response2.IsSuccessStatusCode)
            {
                string data = response2.Content.ReadAsStringAsync().Result;
                chiefList = JsonConvert.DeserializeObject<List<Chief>>(data);
            }
            ViewData["ChiefId"] = new SelectList(chiefList, "Id", "Name", chiefId);
        }
        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            try
            {
                AddAuthorizationHeader();

                if (file == null || file.Length == 0)
                {
                    TempData["errorMessage"] = "Файл не выбран или пуст";
                    return RedirectToAction("Index");
                }



                // Получаем существующие группы и студентов
                var existingGroups = await GetExistingGroups();
                var existingStudents = await GetExistingStudents();

                // Чтение Excel-файла с помощью NPOI
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    IWorkbook workbook;
                    try
                    {
                        workbook = new XSSFWorkbook(stream);
                    }
                    catch
                    {
                        stream.Position = 0;
                        workbook = new HSSFWorkbook(stream);
                    }

                    var worksheet = workbook.GetSheetAt(0);
                    var groupsData = new Dictionary<string, List<StudentViewModel>>();
                    string currentGroupName = null;
                    bool readingStudents = false;

                    for (int rowIndex = 0; rowIndex <= worksheet.LastRowNum; rowIndex++)
                    {
                        var row = worksheet.GetRow(rowIndex);
                        if (row == null) continue;

                        var firstCellValue = row.GetCell(0)?.ToString()?.Trim();

                        if (firstCellValue == "СПИСОК СТУДЕНТОВ")
                        {
                            readingStudents = false;
                            var groupNameRow = worksheet.GetRow(rowIndex + 2);
                            currentGroupName = groupNameRow?.GetCell(0)?.ToString()?.Trim();

                            if (!string.IsNullOrEmpty(currentGroupName))
                            {
                                currentGroupName = currentGroupName.Replace("1 курса группы", "").Trim();

                                if (!existingGroups.ContainsKey(currentGroupName))
                                {
                                    groupsData[currentGroupName] = new List<StudentViewModel>();
                                }

                                rowIndex += 5;
                                readingStudents = true;
                            }
                        }
                        else if (readingStudents && !string.IsNullOrEmpty(currentGroupName))
                        {
                            var studentName = row.GetCell(1)?.ToString()?.Trim();
                            if (!string.IsNullOrEmpty(studentName))
                            {
                                if (existingGroups.TryGetValue(currentGroupName, out var groupId))
                                {
                                    if (!existingStudents.Any(s => s.Name == studentName && s.GroupId == groupId))
                                    {
                                        if (!groupsData.ContainsKey(currentGroupName))
                                        {
                                            groupsData[currentGroupName] = new List<StudentViewModel>();
                                        }
                                        groupsData[currentGroupName].Add(new StudentViewModel
                                        {
                                            Name = studentName,
                                            GroupId = groupId
                                        });
                                    }
                                }
                                else if (groupsData.ContainsKey(currentGroupName))
                                {
                                    groupsData[currentGroupName].Add(new StudentViewModel
                                    {
                                        Name = studentName
                                    });
                                }
                            }
                        }
                    }

                    int importedGroups = 0;
                    int importedStudents = 0;
                    var errors = new List<string>();

                    foreach (var groupData in groupsData)
                    {
                        if (!existingGroups.ContainsKey(groupData.Key))
                        {
                            var group = new GroupViewModel
                            {
                                Name = groupData.Key,
                            };

                            // Отправляем запрос
                            var response = await _client.PostAsJsonAsync($"{_client.BaseAddress}/Group/Post", group);

                            if (response.IsSuccessStatusCode)
                            {
                                importedGroups++;
                                // Если API возвращает только ID в текстовом виде ("Created successfully with ID: 31")
                                var responseText = await response.Content.ReadAsStringAsync();
                                responseText = responseText.Trim('\"');
                                // Извлекаем ID из ответа
                                if (responseText.StartsWith("Created successfully with ID:"))
                                {
                                    var idString = responseText.Split(':').Last().Trim();
                                    if (int.TryParse(idString, out int createdId))
                                    {
                                        // Обновляем ID в существующем объекте группы
                                        group.Id = createdId;

                                        // Теперь group содержит все данные, включая новый ID
                                        // Можно использовать его для дальнейших операций
                                        foreach (var student in groupData.Value)
                                        {
                                            student.GroupId = group.Id; // Привязываем студента к группе
                                            await CreateStudent(student);
                                            importedStudents++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                string errorContent = response.Content.ReadAsStringAsync().Result;

                                TempData["errorMessage"] = $"Error: {response.StatusCode} - {errorContent}";
                                return RedirectToAction("Index");
                            }
                        }
                    }

                    if (errors.Any())
                    {
                        TempData["errorMessages"] = errors;
                    }

                    TempData["successMessage"] = $"Импортировано новых групп: {importedGroups}, новых студентов: {importedStudents}";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"Ошибка при импорте: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult GenerateReport(string sortOrder, string searchGroupName, string searchFacilityName)
        {
            AddAuthorizationHeader();

            // Получаем данные для отчета
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Group/Filter?Name={searchGroupName}&Facility.Name={searchFacilityName}").Result;

            if (!response.IsSuccessStatusCode)
            {
                TempData["errorMessage"] = "Ошибка при получении данных для отчета";
                return RedirectToAction("Index");
            }

            string data = response.Content.ReadAsStringAsync().Result;
            var groups = JsonConvert.DeserializeObject<List<Group>>(data);
            groups = ApplySorting(groups.AsQueryable(), sortOrder).ToList();

            // Создаем книгу Excel
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Группы");

            // Стили для форматирования
            ICellStyle headerStyle = workbook.CreateCellStyle();
            IFont headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);
            headerStyle.Alignment = HorizontalAlignment.Center;

            // Заголовки столбцов
            IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("Название группы");
            //headerRow.CreateCell(1).SetCellValue("Староста");
            headerRow.CreateCell(1).SetCellValue("Факультет");

            // Применяем стиль к заголовкам
            for (int i = 0; i < 2; i++)
            {
                headerRow.GetCell(i).CellStyle = headerStyle;
            }

            // Заполняем данные
            int rowNum = 1;
            foreach (var group in groups)
            {
                IRow row = sheet.CreateRow(rowNum++);
                row.CreateCell(0).SetCellValue(group.Name);
                //row.CreateCell(1).SetCellValue(group.Chief?.Name ?? "-");
                row.CreateCell(1).SetCellValue(group.Facility?.Name ?? "-");
            }

            // Автонастройка ширины столбцов
            for (int i = 0; i < 2; i++)
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
                $"Группы_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        private async Task<HttpResponseMessage> CreateStudent(StudentViewModel student)
        {
            var studentData = JsonConvert.SerializeObject(student);
            var studentContent = new StringContent(studentData, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"{_client.BaseAddress}/Student/Post", studentContent);
        }

        private async Task<Facility?> GetFaisFacility()
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = await _client.GetAsync($"{_client.BaseAddress}/Facility/GetAll");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var facilities = JsonConvert.DeserializeObject<List<Facility>>(data);
                return facilities?.FirstOrDefault(f => f.Name == "ФАИС");
            }

            return null;
        }

        private async Task<Dictionary<string, int>> GetExistingGroups()
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = await _client.GetAsync($"{_client.BaseAddress}/Group/GetAll");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var groups = JsonConvert.DeserializeObject<List<Group>>(data);
                return groups?.ToDictionary(g => g.Name, g => g.Id) ?? new Dictionary<string, int>();
            }

            return new Dictionary<string, int>();
        }

        private async Task<List<StudentViewModel>> GetExistingStudents()
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = await _client.GetAsync($"{_client.BaseAddress}/Student/GetAll");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<StudentViewModel>>(data) ?? new List<StudentViewModel>();
            }

            return new List<StudentViewModel>();
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Domain.ViewModel;
using Presentation.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace Presentation.Controllers
{
    public class DisciplineController : Controller
    {
        Uri baseAddress = new Uri("http://ggtuapi.runasp.net/api");
        private readonly HttpClient _client;

        public DisciplineController()
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
        public IActionResult Index(string sortOrder, string searchString, int? pageNumber, int pageSize = 20)
        {
            AddAuthorizationHeader();

            // Параметры сортировки
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentPageSize"] = pageSize;
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Discipline> DisciplineList = null;
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Discipline/Filter?Name=" + searchString).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                DisciplineList = JsonConvert.DeserializeObject<List<Discipline>>(data).AsQueryable();

                // Применяем сортировку
                switch (sortOrder)
                {
                    case "name_desc":
                        DisciplineList = DisciplineList.OrderByDescending(d => d.Name);
                        break;
                    default:
                        DisciplineList = DisciplineList.OrderBy(d => d.Name);
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

            return View(PaginatedList<Discipline>.Create(DisciplineList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(DisciplineViewModel model)
        {
            try
            {
                AddAuthorizationHeader();
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Discipline/Post", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Discipline created.";
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
                DisciplineViewModel Discipline = new DisciplineViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Discipline/GetDiscipline/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Discipline = JsonConvert.DeserializeObject<DisciplineViewModel>(data);
                }
                return View(Discipline);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(DisciplineViewModel Discipline)
        {
            AddAuthorizationHeader();
            string data = JsonConvert.SerializeObject(Discipline);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/Discipline/Put", content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Discipline updated.";
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
                DisciplineViewModel Discipline = new DisciplineViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Discipline/GetDiscipline/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Discipline = JsonConvert.DeserializeObject<DisciplineViewModel>(data);
                }
                return View(Discipline);
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
                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/Discipline/Delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Discipline deleted.";
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
        public async Task<IActionResult> AddDisciplinesFromFile(IFormFile file)
        {
            try
            {
                AddAuthorizationHeader();

                if (file == null || file.Length == 0)
                {
                    TempData["errorMessage"] = "Файл не выбран или пуст.";
                    return RedirectToAction("Index");
                }

                List<string> existingDisciplines = await LoadExistingDisciplinesAsync();
                if (existingDisciplines == null)
                {
                    TempData["errorMessage"] = "Ошибка при загрузке списка дисциплин из БД.";
                    return RedirectToAction("Index");
                }

                var disciplinesFromFile = ExtractDisciplinesFromFile(file);
                if (disciplinesFromFile.Count == 0)
                {
                    TempData["errorMessage"] = "В файле нет допустимых дисциплин.";
                    return RedirectToAction("Index");
                }

                var newDisciplines = disciplinesFromFile
                    .Where(d => !existingDisciplines.Contains(d, StringComparer.OrdinalIgnoreCase))
                    .Distinct()
                    .ToList();

                // 4. Добавляем новые дисциплины в БД
                if (newDisciplines.Count == 0)
                {
                    TempData["successMessage"] = "Все дисциплины из файла уже есть в базе.";
                    return RedirectToAction("Index");
                }

                foreach (var disciplineName in newDisciplines)
                {
                    var discipline = new DisciplineViewModel { Name = disciplineName };
                    string data = JsonConvert.SerializeObject(discipline);
                    var content = new StringContent(data, Encoding.UTF8, "application/json");

                    var response = await _client.PostAsync(_client.BaseAddress + "/Discipline/Post", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["errorMessage"] = $"Ошибка при добавлении дисциплины: {disciplineName}";
                        return RedirectToAction("Index");
                    }
                }

                TempData["successMessage"] = $"Добавлено {newDisciplines.Count} новых дисциплин.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"Ошибка: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Загружает существующие дисциплины из БД
        private async Task<List<string>> LoadExistingDisciplinesAsync()
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/Discipline/GetAll");
            if (!response.IsSuccessStatusCode) return null;

            string json = await response.Content.ReadAsStringAsync();
            var disciplines = JsonConvert.DeserializeObject<List<Discipline>>(json);
            return disciplines.Select(d => d.Name).ToList();
        }

        // Извлекает дисциплины из файла с фильтрацией
        private List<string> ExtractDisciplinesFromFile(IFormFile file)
        {
            var disciplines = new List<string>();

            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = file.FileName.EndsWith(".xlsx")
                    ? new XSSFWorkbook(stream)
                    : new HSSFWorkbook(stream);

                var sheet = workbook.GetSheetAt(0);

                for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;

                    var cell = row.GetCell(1); // Столбец B
                    if (cell == null) continue;

                    string cellValue = cell.ToString().Trim();
                    if (IsValidDiscipline(cellValue))
                    {
                        disciplines.Add(cellValue);
                    }
                }
            }

            return disciplines;
        }

        // Проверяет, является ли строка валидной дисциплиной
        private bool IsValidDiscipline(string cellValue)
        {
            if (string.IsNullOrWhiteSpace(cellValue))
                return false;

            string[] invalidPatterns =
            {
        "УЧЕБНЫЙ", "Группа", "Специальность", "№ п.п.",
        "Наименование дисциплины", "Учебные занятия",
        "Экзамены", "Практика", "#NULL!", "Недель"
    };

            return !invalidPatterns.Any(p =>
                cellValue.Contains(p, StringComparison.OrdinalIgnoreCase));
        }
    }
}

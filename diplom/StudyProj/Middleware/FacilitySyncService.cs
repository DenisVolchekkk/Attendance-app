using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace StudyProj.Middleware
{
    public class FacilitySyncService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<FacilitySyncService> _logger;
        private readonly TimeSpan _syncInterval = TimeSpan.FromHours(1);

        public FacilitySyncService(IServiceProvider services, ILogger<FacilitySyncService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var primaryDb = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                    var secondaryDb = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

                    await SmartSyncFacilitiesAsync(primaryDb, secondaryDb, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка синхронизации Facilities");
                }

                await Task.Delay(_syncInterval, stoppingToken);
            }
        }

        private async Task SmartSyncFacilitiesAsync(ApplicationContext sourceDb, UsersDbContext targetDb, CancellationToken ct)
        {
            // Получаем записи из обеих баз
            var sourceFacilities = await sourceDb.Facilities.ToListAsync(ct);
            var targetFacilities = await targetDb.Facilities.ToListAsync(ct);

            // Создаем словари для быстрого поиска
            var sourceDict = sourceFacilities.ToDictionary(f => f.Id);
            var targetDict = targetFacilities.ToDictionary(f => f.Id);

            // Добавляем новые и обновляем существующие записи
            foreach (var sourceFacility in sourceFacilities)
            {
                if (targetDict.TryGetValue(sourceFacility.Id, out var targetFacility))
                {
                    // Обновляем существующую запись, если есть изменения
                    if (!AreEqual(sourceFacility, targetFacility))
                    {
                        UpdateFacility(targetFacility, sourceFacility);
                        targetDb.Facilities.Update(targetFacility);
                        _logger.LogInformation($"Обновлен Facility ID: {sourceFacility.Id}");
                    }
                }
                else
                {
                    // Добавляем новую запись
                    targetDb.Facilities.Add(new Facility
                    {
                        Id = sourceFacility.Id,
                        Name = sourceFacility.Name
                        // Копируем остальные свойства
                    });
                    _logger.LogInformation($"Добавлен новый Facility ID: {sourceFacility.Id}");
                }
            }

            // Опционально: удаляем записи, которых нет в источнике
            foreach (var targetFacility in targetFacilities)
            {
                if (!sourceDict.ContainsKey(targetFacility.Id))
                {
                    targetDb.Facilities.Remove(targetFacility);
                    _logger.LogInformation($"Удален Facility ID: {targetFacility.Id}");
                }
            }

            await targetDb.SaveChangesAsync(ct);
            _logger.LogInformation($"Синхронизация завершена. Обработано {sourceFacilities.Count} записей.");
        }

        private bool AreEqual(Facility a, Facility b)
        {
            return a.Name == b.Name; // Добавьте другие свойства для сравнения
        }

        private void UpdateFacility(Facility target, Facility source)
        {
            target.Name = source.Name;
            // Обновите другие свойства по необходимости
        }
    }
}
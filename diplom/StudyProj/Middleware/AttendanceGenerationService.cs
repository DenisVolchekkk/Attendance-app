namespace StudyProj.Middleware.Middleware
{
    public class AttendanceBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AttendanceBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var attendanceService = scope.ServiceProvider.GetRequiredService<AttendanceGeneratorService>();

                    // Генерация посещений для текущего дня
                    await attendanceService.GenerateAttendanceRecordsAsync(DateTime.Today);
                }

                // Ожидание до следующего дня
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }



}

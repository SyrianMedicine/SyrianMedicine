using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class BackgroundTask : BackgroundService
    {
        protected IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundService> _logger;

        public BackgroundTask(IServiceProvider serviceProvider, ILogger<BackgroundService> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // this delay for solve problem when first migration happen 
            await Task.Delay(20000);

            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime datetime = DateTime.Now;
                int hour = datetime.Hour;
                int minute = datetime.Minute;
                int second = datetime.Second;
                if (1 - hour == 0 && 30 - minute == 0)
                {
                    try
                    {
                        _logger.LogInformation("Background services are starting now..");
                        TimeSpan timeSpan = new(0, 0, 60);
                        await Task.Delay(timeSpan, stoppingToken);
                        await CancelReserves();
                        await DeleteUserConnectionTable();
                        await MoveAcceptedReserveForDoctorsAndNursesToHistoryTable();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }
            }
        }

        private async Task MoveAcceptedReserveForDoctorsAndNursesToHistoryTable()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _logger.LogInformation("Move Accepted reserves to history is starting now..");
                    var service = scope.ServiceProvider;
                    var dbContext = service.GetRequiredService<StoreContext>();
                    DateTime dateTime = DateTime.Now;

                    var dbReservesDoctors = await dbContext.ReserveDoctors
                        .Where(e => e.DateTime.Day < dateTime.Day && e.ReserveState == ReserveState.Approved).ToListAsync();
                    foreach (var data in dbReservesDoctors)
                    {
                        DoctorHistory history = new()
                        {
                            Description = data.Description,
                            DoctorId = data.DoctorId,
                            DateTime = data.DateTime,
                            TimeReverse = data.TimeReverse,
                            Title = data.Title,
                            UserId = data.UserId
                        };
                        await dbContext.AddAsync(history);
                    }

                    var dbReservesNurses = await dbContext.ReserveNurses
                        .Where(e => e.DateTime.Day < dateTime.Day && e.ReserveState == ReserveState.Approved).ToListAsync();
                    foreach (var data in dbReservesNurses)
                    {
                        NurseHistory history = new()
                        {
                            Description = data.Description,
                            NurseId = data.NurseId,
                            DateTime = data.DateTime,
                            TimeReverse = data.TimeReverse,
                            Title = data.Title,
                            UserId = data.UserId
                        };
                        await dbContext.NurseHistories.AddAsync(history);
                    }
                    await dbContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        private async Task DeleteUserConnectionTable()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _logger.LogInformation("delete UserConnectionTable is starting now..");
                    var service = scope.ServiceProvider;
                    var dbContext = service.GetRequiredService<StoreContext>();
                    var dbData = await dbContext.UserConnections.ToListAsync();
                    dbContext.RemoveRange(dbData);
                    await dbContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task CancelReserves()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _logger.LogInformation("Cancel Reserves is starting now..");
                    var service = scope.ServiceProvider;
                    var dbContext = service.GetRequiredService<StoreContext>();
                    var datetime = DateTime.Now;
                    var dbReservesDoctors = await dbContext.ReserveDoctors
                        .Where(e => e.ReserveState == ReserveState.Rejected || (e.TimeReverse.Day <= datetime.Day && e.ReserveState == ReserveState.Pending)).ToListAsync();
                    dbContext.ReserveDoctors.RemoveRange(dbReservesDoctors);
                    var dbReservesNurses = await dbContext.ReserveNurses
                        .Where(e => e.ReserveState == ReserveState.Rejected || (e.TimeReverse.Day <= datetime.Day && e.ReserveState == ReserveState.Pending)).ToListAsync();
                    dbContext.ReserveNurses.RemoveRange(dbReservesNurses);
                    var dbReservesHospitals = await dbContext.ReserveHospitals
                        .Where(e => e.ReserveState == ReserveState.Rejected || (datetime.Day - e.DateTime.Day >= 3 && e.ReserveState == ReserveState.Pending)).ToListAsync();
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

    }
}
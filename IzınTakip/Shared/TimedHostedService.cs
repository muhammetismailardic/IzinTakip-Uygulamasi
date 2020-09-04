using IzinTakip.Business.Abstract;
using IzinTakip.DataAccess.Concrete.EntityFramework;
using IzinTakip.Entities.Concrete;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IzinTakip.UI.Shared
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;
        //private readonly IEmployeeService _employeeService;
        //private readonly IEmployeeAnnualDetailsService _employeeAnnualDetailsService;
        private readonly IServiceScopeFactory _scopeFactory;

        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            //_employeeService = employeeService;
            //_employeeAnnualDetailsService = employeeAnnualDetailsService;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}", count);

            CheckIfNewYear(DateTime.Now);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void CheckIfNewYear(DateTime dateTime)
        {
            var newYear = new DateTime((dateTime.Year+1), 1, 1);

            // Check if current date is new year
            if (dateTime >= newYear)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<IzinTakipContext>();
                    var findAllEmployee = dbContext.Employee;

                    foreach (var item in findAllEmployee)
                    {
                        // Burayı Kontrol et.
                        // Check all employees if some of them do not use their annual rights within the last year.

                        if (item.EmployeeAnnualDetails.LastOrDefault().Used == 0)
                        {
                            var details = dbContext.Employee.Where(x => x.Id == item.Id);

                            // if employee does not use his annual right it will be added to total date
                            var annualDetails = new EmployeeAnnualDetails()
                            {
                                EmployeesId = item.Id,
                                AnnualRights = item.YearlyAnnualRightCount,
                                StartDate = DateTime.Now,
                                EndDate = DateTime.Now,
                                Used = 0,
                                LeftDate = item.YearlyAnnualRightCount,
                                IsOnLeave = false
                            };
                            dbContext.EmployeeAnnualDetails.Update(annualDetails);
                        }

                        var recDate = item.RecruitmentDate.Year;
                        int currentDate = DateTime.Now.Year;
                        int value = currentDate - recDate;

                        if (value < 1)
                        {
                            item.YearlyAnnualRightCount = 0;
                        }
                        else if (value >= 1 && value <= 5)
                        {
                            item.YearlyAnnualRightCount = 14;
                        }
                        else if (value > 5 && value >= 10)
                        {
                            item.YearlyAnnualRightCount = 21;
                        }
                        else
                        {
                            item.YearlyAnnualRightCount = 30;
                        }
                    }
                }
            }
        }
    }
}

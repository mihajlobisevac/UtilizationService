using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using UtilizationService.Common;
using UtilizationService.Database;
using UtilizationService.Models;
using UtilizationService.Services;

namespace UtilizationService
{
    [SupportedOSPlatform("windows")]
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await AddCpuRecordToDb(HardwareExtensions.GetCpuUsage(), "Win32_Processor");
                await AddMemoryRecordToDb(HardwareExtensions.GetAvailableRamInMB(), "Win32_PhysicalMemory");
                await AddDiskRecordToDb(HardwareExtensions.GetDiskUsage("_Total"), "Win32_DiskDrive");

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(5*60*1000, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var hardwareList = HardwareService.GetHardwareInfo();

            AddHardwareToDb(hardwareList);

            return base.StartAsync(cancellationToken);
        }

        private async Task AddCpuRecordToDb(float cpuUsage, string type)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var cpu = await context.HardwareTypes.FirstOrDefaultAsync(x => x.Type == type);
            if (cpu is not null)
            {
                var record = new Record(cpu, Convert.ToInt32(cpuUsage));

                context.Records.Add(record);
                context.SaveChanges();
            }
        }

        private async Task AddMemoryRecordToDb(float availalbeInMB, string type)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var physicalMemory = await context.HardwareTypes.Where(x => x.Type == type).ToListAsync();
            if (physicalMemory is not null && physicalMemory.Count > 0)
            {
                var listOfRecords = new List<Record>();

                foreach (var memory in physicalMemory)
                {
                    listOfRecords.Add(new Record(memory, Convert.ToInt32(availalbeInMB)));
                }

                context.Records.AddRange(listOfRecords);
                context.SaveChanges();
            }
        }

        private async Task AddDiskRecordToDb(float diskUsage, string type)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var disks = await context.HardwareTypes.Where(x => x.Type == type).ToListAsync();
            if (disks is not null && disks.Count > 0)
            {
                var listOfRecords = new List<Record>();

                foreach (var disk in disks)
                {
                    listOfRecords.Add(new Record(disk, Convert.ToInt32(diskUsage)));
                }

                context.Records.AddRange(listOfRecords);
                context.SaveChanges();
            }
        }

        private void AddHardwareToDb(List<HardwareType> hardwareList)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            foreach (var hardware in hardwareList)
            {
                if (context.HardwareTypes.Any(x => x.Identifier == hardware.Identifier) == false)
                {
                    context.HardwareTypes.Add(hardware);
                }

                context.SaveChanges();
            }
        }
    }
}

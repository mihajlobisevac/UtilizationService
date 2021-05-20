using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilizationService.Database;
using UtilizationService.Models;

namespace UtilizationService.Common
{
    public class Report
    {
        private readonly IServiceProvider _serviceProvider;

        public Report(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Create(DateTime date)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var hardwareTypes = await context.HardwareTypes.ToListAsync();

            var records = await context.Records
                .Include(x => x.HardwareType)
                .Where(x => x.CreateDate.Date == date.Date)
                .ToListAsync();

            if (records is not null &&
                records.Count > 0 &&
                hardwareTypes is not null &&
                hardwareTypes.Count > 0)
            {
                var recordList = CreateRecordList(hardwareTypes, records, date);

                CreateTextFileAsync(recordList, date);
            }
        }

        private static List<ReportDto> CreateRecordList(List<HardwareType> hardwareTypes, List<Record> records, DateTime date)
        {
            var reportList = new List<ReportDto>();

            foreach (var hardware in hardwareTypes)
            {
                var values = records.Where(x => x.HardwareType.Type == hardware.Type)
                    .Select(x => x.Value)
                    .ToList();

                var averageValue = values.Count > 0 ? values.Average() : 0;

                var report = new ReportDto
                {
                    Value = Convert.ToInt32(averageValue),
                    CreateDate = date,
                    Model = hardware.Model,
                    AdditionalInfo = hardware.AdditionalInfo
                };

                reportList.Add(report);
            }

            return reportList;
        }

        private static void CreateTextFileAsync(List<ReportDto> reportList, DateTime date)
        {
            var dateTime = $"{date.Year}-{date.Month}-{date.Day}";

            var path = @"E:\Report_" + dateTime + ".txt";

            try
            {
                if (File.Exists(path))
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                    File.Delete(path);
                }

                using (FileStream fs = File.Create(path))
                {
                    byte[] header = new UTF8Encoding(true).GetBytes(
                        $"This report displays the average utilization value of certain components during the date: {dateTime}" +
                        Environment.NewLine + Environment.NewLine + Environment.NewLine);

                    fs.Write(header);

                    foreach (var report in reportList)
                    {
                        byte[] line = new UTF8Encoding(true).GetBytes(
                            $"Model: {report.Model}" + Environment.NewLine +
                            $"Average Value: {report.Value}" + Environment.NewLine +
                            $"Additional Info: {report.AdditionalInfo}" + Environment.NewLine +
                            $"Create Date: {report.CreateDate}" + 
                            Environment.NewLine + Environment.NewLine);

                        fs.Write(line);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

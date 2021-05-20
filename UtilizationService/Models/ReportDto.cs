using System;

namespace UtilizationService.Models
{
    public class ReportDto
    {
        public int Value { get; set; }
        public DateTime CreateDate { get; set; }
        public string Model { get; set; }
        public string AdditionalInfo { get; set; }
    }
}

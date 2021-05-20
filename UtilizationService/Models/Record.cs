using System;

namespace UtilizationService.Models
{
    public class Record
    {
        public Guid Id { get; set; }
        public HardwareType HardwareType { get; set; }
        public int Value { get; set; }
        public DateTime CreateDate { get; set; }

        public Record(HardwareType hardwareType, int value)
        {
            Id = Guid.NewGuid();
            HardwareType = hardwareType;
            Value = value;
            CreateDate = DateTime.Now;
        }

        protected Record()
        {
        }
    }
}

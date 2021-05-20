using System;
using System.ComponentModel.DataAnnotations;

namespace UtilizationService.Models
{
    public class HardwareType
    {
        public Guid Id { get; set; }

        [MaxLength(100)] 
        public string Model { get; set; }
        
        [MaxLength(300)]
        public string AdditionalInfo { get; set; }

        [MaxLength(100)]
        public string Identifier { get; set; }

        [MaxLength(100)]
        public string Type { get; set; }

        public HardwareType(string model, string identifier, string type, string additionalInfo)
        {
            Id = Guid.NewGuid();
            Model = model;
            Identifier = identifier;
            Type = type;
            AdditionalInfo = additionalInfo;
        }

        protected HardwareType()
        {
        }
    }
}

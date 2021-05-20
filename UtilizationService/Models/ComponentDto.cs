using System.Collections.Generic;

namespace UtilizationService.Models
{
    public class ComponentDto
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }
        public List<string> AdditionalInfo { get; set; }

        public ComponentDto(string name, string identifier, string type, List<string> additionalInfo)
        {
            Name = name;
            Identifier = identifier;
            Type = type;
            AdditionalInfo = additionalInfo;
        }
    }
}

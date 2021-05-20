using System.Collections.Generic;
using System.Management;
using System.Runtime.Versioning;
using UtilizationService.Common;
using UtilizationService.Models;

namespace UtilizationService.Services
{
    [SupportedOSPlatform("windows")]
    public class HardwareService
    {
        public static List<ComponentDto> GetComponents(
            string hardwareClass,
            string nameSyntax,
            string identifierSyntax,
            List<string> additionalInfoSyntaxList)
        {
            var mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + hardwareClass);

            var components = mos.Get()
                .ConvertToComponentList(
                    hardwareClass,
                    nameSyntax, 
                    identifierSyntax, 
                    additionalInfoSyntaxList);

            return components;
        }

        public static List<HardwareType> GetHardwareInfo()
        {
            var componentList = new List<List<ComponentDto>>()
            {
                GetComponents("Win32_Processor", "Name", "SerialNumber", new List<string> { "Architecture", "Manufacturer" }),
                GetComponents("Win32_DiskDrive", "Caption", "SerialNumber", new List<string> { "Size", "Manufacturer" }),
                GetComponents("Win32_PhysicalMemory", "Name", "SerialNumber", new List<string> { "Speed", "Capacity", "Manufacturer" }),
            };

            return componentList.ToHardwareTypeList();
        }
    }
}

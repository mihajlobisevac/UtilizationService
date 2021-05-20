using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.Versioning;
using System.Threading;
using UtilizationService.Models;

namespace UtilizationService.Common
{
    [SupportedOSPlatform("windows")]
    public static class HardwareExtensions
    {
        public static List<HardwareType> ToHardwareTypeList(this List<List<ComponentDto>> components)
        {
            var hardwareList = new List<HardwareType>();

            foreach (var componentTypeList in components)
            {
                foreach (var hardware in componentTypeList)
                {
                    var hardwareType = new HardwareType(
                        hardware.Name,
                        hardware.Identifier,
                        hardware.Type,
                        hardware.AdditionalInfo.CombineBy(", "));

                    hardwareList.Add(hardwareType);
                }
            }

            return hardwareList;
        }

        [SupportedOSPlatform("windows")]
        public static List<ComponentDto> ConvertToComponentList(
            this ManagementObjectCollection moc,
            string hardwareClass,
            string nameSyntax,
            string identifierSyntax,
            List<string> additionalInfoSyntaxList)
        {
            var componenetList = new List<ComponentDto>();
            int counterHash = 0;

            foreach (ManagementObject item in moc)
            {
                counterHash++;
                var name = Convert.ToString(item[nameSyntax]);
                var identifier = Convert.ToString(item[identifierSyntax]) + counterHash;
                var additionalInfo = new List<string>();

                foreach (var additInfo in additionalInfoSyntaxList)
                {
                    additionalInfo.Add(item.HumanizeAdditionalInfo(additInfo));
                }

                componenetList.Add(new ComponentDto(name, identifier, hardwareClass, additionalInfo));
            }

            return componenetList;
        }

        public static float GetCpuUsage()
        {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            return GetValue(cpuCounter);
        }

        public static float GetAvailableRamInMB()
        {
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            return GetValue(ramCounter);
        }

        public static float GetDiskUsage(string instanceName)
        {
            var diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", instanceName);
            return GetValue(diskCounter);
        }

        private static float GetValue(PerformanceCounter counter)
        {
            counter.NextValue();
            Thread.Sleep(500);
            return counter.NextValue();
        }
    }
}

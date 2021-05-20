using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.Versioning;

namespace UtilizationService.Common
{
    public static class GeneralExtensions
    {
        [SupportedOSPlatform("windows")]
        public static string HumanizeAdditionalInfo(this ManagementObject managementObject, string additInfo)
        {
            return $"{additInfo}: {Convert.ToString(managementObject[additInfo])}";
        }

        public static string CombineBy(this List<string> listOfStrings, string separator)
        {
            return string.Join(separator, listOfStrings);
        }
    }
}

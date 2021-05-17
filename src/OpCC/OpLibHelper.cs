using System;
using System.Diagnostics;

namespace OpLib
{
    public static class Helper
    {
        public static string AddDEVToVersion(string versionNumber, bool launchedFromStudio)
        {
            if (launchedFromStudio)
            {
                versionNumber = string.Concat(versionNumber, ".[DEV]");
            }
            return versionNumber;
        }

        public static string AddRDToVersion(string versionNumber)
        {
#if DEBUG
            versionNumber = string.Concat(versionNumber, ".D");
#else
            versionNumber = string.Concat(versionNumber, ".R");
#endif
            return versionNumber;
        }

        //        public static void AddRDToVersion()
        //        {
        //#if DEBUG
        //            VersionNumber += ".D";
        //#else
        //            VersionNumber+=".R";
        //#endif
        //        }

        public static string AddSystemInfo(string systemInfo)
        {
            if (!Environment.Is64BitProcess)
            {
                systemInfo = "32Bit";
            }
            else
            {
                systemInfo = "64Bit";
            }
            systemInfo = string.Concat(systemInfo, ", ", Environment.Version.ToString());
            systemInfo = string.Concat(systemInfo, ", OS:");
            if (!Environment.Is64BitOperatingSystem)
            {
                systemInfo = string.Concat(systemInfo, "32Bit");
            }
            else
            {
                systemInfo = string.Concat(systemInfo, "64Bit");
            }
            systemInfo = string.Concat(systemInfo, ", ", Environment.OSVersion.ToString());
            return systemInfo;
        }

        public static bool CheckLaunchedFromStudio()
        {
            Process.GetCurrentProcess().MainModule.ModuleName.Contains(".vshost");
            return Debugger.IsAttached;
        }

        public static string FormatDate(DateTime date, string format)
        {
            return date.ToString(format).Replace("..", ".");
        }
    }
}

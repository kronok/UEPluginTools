using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace UEPluginPackager
{

    public struct PluginVersionNumber
    {
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int PatchVersion { get; set; }

        public PluginVersionNumber()
        {
            MajorVersion = 0;
            MinorVersion = 0;
            PatchVersion = 0;
        }
    }

    public struct PlatformVersionInfo
    {
        public string Platform { get; set; }
        public string UnrealVersion { get; set; }
        public PluginVersionNumber Version { get; set; }
        public string DownloadURL { get; set; }

        public PlatformVersionInfo()
        {
            Platform = WindowsPlatform;
            UnrealVersion = "0.0";
            Version = new PluginVersionNumber();
            DownloadURL = "https://www.patreon.com/c/Kronok";
        }

        public PlatformVersionInfo(string PlatformString, string UnrealVersionString, PluginVersionNumber VersionNum, string DownloadURLString)
        {
            Platform = PlatformString;
            UnrealVersion = UnrealVersionString;
            Version = VersionNum;
            DownloadURL = DownloadURLString;
        }

        static public string WindowsPlatform = "Windows";
        static public string LinuxPlatform = "Linux";
        static public string OSXPlatform = "OSX";
        static public string DefaultURL = "https://www.patreon.com/c/Kronok";
    }


    public struct PluginVersionInfo
    {
        public string PluginName { get; set; }
        public string PluginURL { get; set; }
        public string ReleaseNotesURL { get; set; }

        public List<PlatformVersionInfo> Platforms;

        public PluginVersionInfo(string PluginNameIn, string PluginURLIn)
        {
            PluginName = PluginNameIn;
            PluginURL = PluginURLIn;

            Platforms = new List<PlatformVersionInfo>();

            ReleaseNotesURL = DefaultReleaseNotesURL;
        }

        static public string DefaultReleaseNotesURL = "https://www.patreon.com/c/Kronok";
    }


    internal class UEPluginVersionUtils
    {
        public static string VersionSetToJSON(PluginVersionInfo VersionSet)
        {
            JsonSerializerOptions Options = new JsonSerializerOptions();
            Options.IncludeFields = true;
            Options.WriteIndented = true;
            return JsonSerializer.Serialize(VersionSet, Options);
        }
    }
}

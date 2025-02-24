// See https://aka.ms/new-console-template for more information
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using UEPluginPackager;

bool bDeletePrivateSource = true;
bool bDeletePublicSource = true;
bool bDeletePDBFiles = false;
bool bDeletePatchFiles = true;

bool bDeleteDevelopment = false;
bool bDeleteShipping = false;
if (args.Contains("-NoShipping", StringComparer.OrdinalIgnoreCase))
    bDeleteShipping = true;

bool bEnableByDefault = false;
if (args.Contains("-EnableByDefault", StringComparer.OrdinalIgnoreCase))
    bEnableByDefault = true;


//
// TODO: currently this is desctructive....would be nice to copy everything to a temp folder and
// work there...
//


string PluginRootPath = args[0];
if (!Directory.Exists(PluginRootPath))
{
    Console.WriteLine("Invalid Plugin Root Path " + PluginRootPath);
    return;
}
string PluginDirName = new System.IO.DirectoryInfo(PluginRootPath).Name;

Console.WriteLine("Packaging plugin " + PluginDirName + " in path " + PluginRootPath + "...");


//
// Update the ModuleName.build.cs files in the plugin source folders to
// contain the line 'bUsePrecompiled=true;'. This is done by searching each line
// for the string "//#UEPLUGINTOOL" and replacing it w/ the precompiled line
//
Console.WriteLine("  ...Updating build.cs files...");
int NumBuildFilesUpdated = UEPluginPackager.UEPluginCleanupUtils.UpdateBuildCSFiles(PluginRootPath);
if ( NumBuildFilesUpdated == 0 )
{
    Console.WriteLine("!!WARNING!! no build.cs files were found to contain the //#UEPLUGINTOOL tag, plugin may not function correctly");
}

Console.WriteLine("  ...Updating uplugin files...");
UEPluginPackager.UEPluginCleanupUtils.UpdateUPluginFile(PluginRootPath, bEnableByDefault);

//
// Remove Private and Public folders inside Source directories (but not build.cs files)
// This is optional. Removing Private will prevent the plugin from being recompiled.
// Removing Public will prevent the plugin from being used in (eg) C++ subclasses, etc
//
if (bDeletePrivateSource || bDeletePublicSource)
{
    Console.WriteLine("  ...Deleting source files...");
    UEPluginPackager.UEPluginCleanupUtils.DeleteSourceFiles(PluginRootPath, bDeletePrivateSource, bDeletePublicSource);
}

//
// Remove PDB files. This will prevent debugging (which would still require private source files, anyway)
//
if (bDeletePDBFiles)
{
    Console.WriteLine("  ...Deleting PDB Files...");
    UEPluginPackager.UEPluginCleanupUtils.DeletePDBFiles(PluginRootPath);
}

//
// Remove Patch files
//
if ( bDeletePatchFiles )
{
    Console.WriteLine("  ...Deleting Patch Files...");
    UEPluginPackager.UEPluginCleanupUtils.DeletePatchFiles(PluginRootPath);
}

if ( bDeleteDevelopment || bDeleteShipping )
{
    Console.WriteLine("  ...Deleting Intermediate Files...");
    UEPluginPackager.UEPluginCleanupUtils.DeleteIntermediateFiles(PluginRootPath, bDeleteDevelopment, bDeleteShipping);
}




//
// Create output zip archive and version-info text files
//

// read version number file
// This file should be in <PluginName>/Source/SCYTHE_VERSION_NUMBER.txt

PluginVersionNumber Version = new PluginVersionNumber();
string VersionNumberFilePath = Path.Combine(PluginRootPath, "Source", "SCYTHE_VERSION_NUMBER.txt");
if (File.Exists(VersionNumberFilePath))
{
    string VersionText = File.ReadAllText(VersionNumberFilePath);
    string[] VersionTokens = VersionText.Split('.');
    try
    {
        Version.MajorVersion = int.Parse(VersionTokens[0]);
        Version.MinorVersion = int.Parse(VersionTokens[1]);
        Version.PatchVersion = int.Parse(VersionTokens[2]);
    }
    catch
    {
        Console.WriteLine("!!WARNING!! version string in file " + VersionNumberFilePath + " is malformed: [" + VersionText + "] - should be in format Major.Minor.Patch. Ignoring...");
    }
}
else
{
    Console.WriteLine("!!WARNING!! no version file exist at " + VersionNumberFilePath + ", using default version");
}

// read Unreal version number file
// this file should be in <PluginName>/Source/SCYTHE_UNREAL_VERSION.txt
string UnrealVersion = "0.0";
string UnrealVersionFilePath = Path.Combine(PluginRootPath, "Source", "SCYTHE_UNREAL_VERSION.txt");
if (File.Exists(UnrealVersionFilePath))
{
    string FileText = File.ReadAllText(UnrealVersionFilePath);
    string[] VersionTokens = FileText.Split('.');
    try
    {
        if (VersionTokens.Length == 2)
            UnrealVersion = int.Parse(VersionTokens[0]).ToString() + '.' + int.Parse(VersionTokens[1]).ToString();
        else if (VersionTokens.Length == 2)
            UnrealVersion = int.Parse(VersionTokens[0]).ToString() + '.' + int.Parse(VersionTokens[1]).ToString() + '.' + int.Parse(VersionTokens[2]).ToString();
    }
    catch
    {
        Console.WriteLine("!!WARNING!! version string in file " + UnrealVersionFilePath + " is malformed: [" + FileText + "] - should be in format Major.Minor or Major.Minor.Patch Ignoring...");
    }
}
else
{
    Console.WriteLine("!!WARNING!! no Unreal version file exist at " + UnrealVersionFilePath + ", using default version 0.0");
}


string PluginVersionSuffix = String.Format("_{0}_{1}_{2}", Version.MajorVersion, Version.MinorVersion, Version.PatchVersion);
string Win64PlatformSuffix = "_Win64";
string LinuxPlatformSuffix = "_Linux";
string OSXPlatformSuffix = "_OSX";

string PluginURL = "https://www.patreon.com/c/Kronok";
string DownloadURLRoot = "";
string DownloadURL_Win64 = "https://www.patreon.com/c/Kronok";
// string DownloadURL_Linux = DownloadURLRoot + PluginDirName + "/" + UnrealVersion + "/" + PluginDirName + PluginVersionSuffix + LinuxPlatformSuffix + ".zip";
// string DownloadURL_OSX = DownloadURLRoot + PluginDirName + "/" + UnrealVersion + "/" + PluginDirName + PluginVersionSuffix + OSXPlatformSuffix + ".zip";

// construct PluginVersionInfo struct that will be converted to JSON and saved both
// inside the plugin at <PluginName>/SCYTHE_VERSION_INFO.txt and in ..\<PluginName>_VERSION.txt.

PluginVersionInfo PluginVersionInfo = new PluginVersionInfo(PluginDirName, PluginURL);
PluginVersionInfo.Platforms.Add(
    new PlatformVersionInfo(PlatformVersionInfo.WindowsPlatform, UnrealVersion, Version, DownloadURL_Win64));
// PluginVersionInfo.Platforms.Add(
//     new PlatformVersionInfo(PlatformVersionInfo.LinuxPlatform, UnrealVersion, Version, DownloadURL_Linux));
// PluginVersionInfo.Platforms.Add(
//     new PlatformVersionInfo(PlatformVersionInfo.OSXPlatform, UnrealVersion, Version, DownloadURL_OSX));
string VersionJSON = UEPluginVersionUtils.VersionSetToJSON(PluginVersionInfo);


string VersionFilePath = Path.Combine(PluginRootPath, "SCYTHE_VERSION_INFO.txt");
File.WriteAllText(VersionFilePath, VersionJSON);


string BaseDirPath = Path.Combine(PluginRootPath, "..");
string ZipFilePath = Path.Combine(BaseDirPath, PluginDirName + PluginVersionSuffix + Win64PlatformSuffix + ".zip");

// delete existing zip file
if ( File.Exists(ZipFilePath) )
{
    Console.WriteLine("  ...deleting existing zip archive " + ZipFilePath + "...");
    File.Delete(ZipFilePath);
}

// Copy LICENSE.txt and README.txt to the plugin root directory
string licenseFilePath = Path.Combine(PluginRootPath, "LICENSE.txt");
string readmeFilePath = Path.Combine(PluginRootPath, "README.txt");

File.Copy(Path.Combine(PluginRootPath, "..", "..", "PluginSources", "LICENSE.txt"), licenseFilePath, true);
File.Copy(Path.Combine(PluginRootPath, "..", "..", "PluginSources", "README.txt"), readmeFilePath, true);



// create new zip file at path <PluginName>/../<PluginName>.zip
Console.WriteLine("  ...Creating zip archive " + ZipFilePath + "...");
ZipFile.CreateFromDirectory(PluginRootPath, ZipFilePath, CompressionLevel.Optimal, true);

// create new version info file at paths:
//   <PluginName>/../<PluginName>_CURRENTVERSION.txt
//   <PluginName>/../<PluginName>_<Major>_<Minor>_<Patch>.txt
string ZipFileVersionPath = Path.Combine(BaseDirPath, PluginDirName + PluginVersionSuffix + ".txt");
File.WriteAllText(ZipFileVersionPath, VersionJSON);
string CurrentVersionPath = Path.Combine(BaseDirPath, PluginDirName + "_CURRENTVERSION.txt");
File.WriteAllText(CurrentVersionPath, VersionJSON);
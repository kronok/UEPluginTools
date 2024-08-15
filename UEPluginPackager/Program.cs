﻿// See https://aka.ms/new-console-template for more information
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using UEPluginPackager;

bool bDeletePrivateSource = true;
bool bDeletePublicSource = true;
bool bDeletePDBFiles = true;
bool bDeletePatchFiles = true;

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

//
// Remove intermediate files (todo make this more specific)
//
//Console.WriteLine("Deleting Intermediate files...");
//UEPluginPackager.UEPluginCleanupUtils.DeleteIntermediateFiles(PluginRootPath);


//
// Create output zip archive and version-info text files
//

// read version number file
// This file should be in <PluginName>/Source/GS_VERSION_NUMBER.txt

PluginVersionNumber Version = new PluginVersionNumber();
string VersionNumberFilePath = Path.Combine(PluginRootPath, "Source", "GS_VERSION_NUMBER.txt");
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
        Console.WriteLine("!!WARNING!! version string in file " + VersionNumberFilePath + " is malformed: [" + VersionText + "] - should be informat Major.Minor.Patch. Ignoring...");
    }
}
else
{
    Console.WriteLine("!!WARNING!! no version file exist at " + VersionNumberFilePath + ", using default version");
}


string PluginVersionSuffix = String.Format("_{0}_{1}_{2}", Version.MajorVersion, Version.MinorVersion, Version.PatchVersion);

string PluginURL = "https://www.gradientspace.com/uetoolbox";
string DownloadURLRoot = "https://dg1t4a32yvqmr.cloudfront.net/";
string DownloadURL = DownloadURLRoot + PluginDirName + "/" + PluginDirName + PluginVersionSuffix + ".zip";


// construct PluginVersionInfo struct that will be converted to JSON and saved both
// inside the plugin at <PluginName>/GS_VERSION_INFO.txt and in ..\<PluginName>_VERSION.txt.

PluginVersionInfo PluginVersionInfo = new PluginVersionInfo(PluginDirName, PluginURL);
PluginVersionInfo.Platforms.Add(
    new PlatformVersionInfo(PlatformVersionInfo.WindowsPlatform, Version, DownloadURL));
PluginVersionInfo.Platforms.Add(
    new PlatformVersionInfo(PlatformVersionInfo.LinuxPlatform, Version, DownloadURL));
PluginVersionInfo.Platforms.Add(
    new PlatformVersionInfo(PlatformVersionInfo.OSXPlatform, Version, DownloadURL));
string VersionJSON = UEPluginVersionUtils.VersionSetToJSON(PluginVersionInfo);


string VersionFilePath = Path.Combine(PluginRootPath, "GS_VERSION_INFO.txt");
File.WriteAllText(VersionFilePath, VersionJSON);

string BaseDirPath = Path.Combine(PluginRootPath, "..");
string ZipFilePath = Path.Combine(BaseDirPath, PluginDirName + PluginVersionSuffix + ".zip");

// delete existing zip file
if ( File.Exists(ZipFilePath) )
{
    Console.WriteLine("  ...deleting existing zip archive " + ZipFilePath + "...");
    File.Delete(ZipFilePath);
}

// create new zip file at path <PluginName>/../<PluginName>.zip
Console.WriteLine("  ...Creating zip archive " + ZipFilePath + "...");
ZipFile.CreateFromDirectory(PluginRootPath, ZipFilePath);

// create new version info file at path <PluginName>/../<PluginName>_VERSION.txt
string ZipFileVersionPath = Path.Combine(BaseDirPath, PluginDirName + "_VERSION.txt");
File.WriteAllText(ZipFileVersionPath, VersionJSON);
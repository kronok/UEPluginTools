// See https://aka.ms/new-console-template for more information
using System.IO.Compression;

Console.WriteLine("Hello, World!");


string PluginRootPath = args[0];
if (!Directory.Exists(PluginRootPath))
{
    Console.WriteLine("Invalid Plugin Root Path " + PluginRootPath);
    return;
}

Console.WriteLine("Updating build.cs files...");
UEPluginPackager.UEPluginCleanupUtils.UpdateBuildCSFiles(PluginRootPath);

Console.WriteLine("Deleting source files...");
UEPluginPackager.UEPluginCleanupUtils.DeleteSourceFiles(PluginRootPath, true, true);

Console.WriteLine("Deleting PDB Files...");
UEPluginPackager.UEPluginCleanupUtils.DeletePDBFiles(PluginRootPath);

Console.WriteLine("Deleting Intermediate files...");
UEPluginPackager.UEPluginCleanupUtils.DeleteIntermediateFiles(PluginRootPath);

Console.WriteLine("Creating zip archive...");
ZipFile.CreateFromDirectory(PluginRootPath,
    Path.Combine(Path.Combine(PluginRootPath, ".."), PluginRootPath + ".zip"));
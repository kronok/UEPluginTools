using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UEPluginPackager
{
    internal class UEPluginCleanupUtils
    {
        public static int UpdateBuildCSFiles(string PluginRootPath)
        {
            string[] BuildFiles = Directory.GetFiles(PluginRootPath, "*.build.cs", SearchOption.AllDirectories);
            int NumUpdated = 0;

            foreach (string BuildFilePath in BuildFiles)
            {
                Console.WriteLine("      Processing " + BuildFilePath);

                bool bFoundPluginToolLine = false;
                string[] Lines = File.ReadAllLines(BuildFilePath);
                int NumLines = Lines.Length;
                for (int li = 0; li < NumLines; li++)
                {
                    if (Lines[li].Contains("//#UEPLUGINTOOL"))
                    {
                        Lines[li] = Lines[li].Replace("//#UEPLUGINTOOL", "bUsePrecompiled=true;");
                        bFoundPluginToolLine = true;
                    }
                }

                File.WriteAllLines(BuildFilePath, Lines);

                if (!bFoundPluginToolLine)
                {
                    Console.WriteLine("        *** Did not find //#UEPLUGINTOOL token!!");
                }
                else
                {
                    NumUpdated++;
                }
            }
            return NumUpdated;
        }



        public static void DeleteSourceFiles(string PluginRootPath, bool bPrivate, bool bPublic)
        {
            string SourcePath = Path.Combine(PluginRootPath, "Source");
            string[] ModuleFolders = Directory.GetDirectories(SourcePath);
            foreach (string ModuleFolder in ModuleFolders)
            {
                Console.WriteLine("  Processing " + ModuleFolder);

                if (bPrivate)
                {
                    string PrivatePath = Path.Combine(ModuleFolder, "Private");
                    if (Directory.Exists(PrivatePath))
                    {
                        Console.WriteLine("    Deleting " + PrivatePath);
                        Directory.Delete(PrivatePath, true);
                    }
                }

                if (bPublic)
                {
                    string PublicPath = Path.Combine(ModuleFolder, "Public");
                    if (Directory.Exists(PublicPath))
                    {
                        Console.WriteLine("    Deleting " + PublicPath);
                        Directory.Delete(PublicPath, true);
                    }
                }
            }
        }



        public static void DeletePDBFiles(string PluginRootPath)
        {
            string BinariesPath = Path.Combine(PluginRootPath, "Binaries");
            string[] PDBFiles = Directory.GetFiles(PluginRootPath, "*.pdb", SearchOption.AllDirectories);
            foreach (string PDBFile in PDBFiles)
            {
                File.Delete(PDBFile);
            }
        }


        public static void DeletePatchFiles(string PluginRootPath)
        {
            string BinariesPath = Path.Combine(PluginRootPath, "Binaries");
            string[] PDBFiles = Directory.GetFiles(PluginRootPath, "*.patch_*.*", SearchOption.AllDirectories);
            foreach (string PDBFile in PDBFiles)
            {
                File.Delete(PDBFile);
            }
        }


        public static void DeleteIntermediateFiles(string PluginRootPath)
        {
            // todo...
        }
    }
}

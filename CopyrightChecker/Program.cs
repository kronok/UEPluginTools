namespace CopyrightChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string CopyrightString = "// Copyright Gradientspace Corp. All Rights Reserved.";

            if (args.Length == 0 )
            {
                System.Console.WriteLine("Usage: CopyrightChecker <SourcePath> [-AddIfMissing]");
                return;
            }

            // just have to put this in CWD
            string SourceRootPath = args[0];

            bool bEnableAddIfMissing = args.Contains("-AddIfMissing");

            string[] CsCodeFiles = Directory.GetFiles(SourceRootPath, "*.cs", SearchOption.AllDirectories);
            string[] CppCodeFiles = Directory.GetFiles(SourceRootPath, "*.cpp", SearchOption.AllDirectories);
            string[] CppHeaderFiles = Directory.GetFiles(SourceRootPath, "*.h", SearchOption.AllDirectories);

            List<string[]> FileSetsToProcess = new List<string[]>();
            FileSetsToProcess.Add(CppCodeFiles);
            FileSetsToProcess.Add(CppHeaderFiles);

            foreach (string[] FileSet in FileSetsToProcess)
            {
                foreach (string FilePath in FileSet)
                {
                    string[] Lines = File.ReadAllLines(FilePath);
                    if (Lines[0].StartsWith(CopyrightString) == false)
                    {
                        if (bEnableAddIfMissing)
                        {
                            List<string> NewLines = new List<string>();
                            NewLines.Add(CopyrightString);
                            NewLines.AddRange(Lines);
                            File.WriteAllLines(FilePath, NewLines);
                        }
                        else
                        {
                            System.Console.WriteLine(FilePath);
                        }
                    }
                }
            }
        }
    }
}

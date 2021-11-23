using System;
using System.IO;
using System.Text;

namespace FindNewlines
{
    class Program
    {
        private static string s_baseDir = @"C:\dev\psesdk\src";

        private static string GetRelativeFilename(
            string filename)
        {
            if (filename.StartsWith(s_baseDir, StringComparison.CurrentCultureIgnoreCase))
                return filename.Substring(s_baseDir.Length).TrimStart('\\', '/');
            return filename;
        }

        static void Main(string[] args)
        {
            LineEndingType okType = LineEndingType.CRLF;
            int filesFound = ScanDirectory(s_baseDir, okType, 0);
            Console.WriteLine("Number of files with line endings different from {0}: {1}", okType, filesFound);
        }

        private static int ScanDirectory(
            string dir,
            LineEndingType okType,
            int depth)
        {
            if (depth == 1)
                LogMessage($"Scanning folder {GetRelativeFilename(dir)}...");
            int numFound = 0;
            foreach (string filename in Directory.EnumerateFiles(dir))
            {
                switch (Path.GetExtension(filename)?.ToLower())
                {
                    case ".cs":
                    case ".xml":
                    case ".xaml":
                    case ".cpp":
                    case ".cxx":
                    case ".hpp":
                    case ".h":
                    case ".c":
                    case ".sln":
                    case ".csproj":
                    case ".htm":
                    case ".html":
                        if (CheckFile(filename, okType))
                        {
                            numFound++;
                        }
                        break;

                }
            }

            foreach (string subdir in Directory.EnumerateDirectories(dir))
            {
                if (ShouldSkipFolder(subdir))
                    continue;
                
                numFound += ScanDirectory(subdir, okType, depth + 1);
            }
            
            return numFound;
        }

        private static bool ShouldSkipFolder(string dir)
        {
            string dirName = Path.GetFileName(dir);
            if (dir == null)
                // Shouldn't happen...
                return true;
            if (dirName.StartsWith('.'))

                // Hidden directory
                return true;
            if (dir == "bin")
                return true;
            if (dir == "obj")
                return true;

            return false;
        }

        private static bool CheckFile(
            string filename, 
            LineEndingType okType)
        {
            var type = GetLineEndingType(filename);
            if (type != okType && type != LineEndingType.None)
            {
                PrintMatch(filename, type);
                return true;
            }

            return false;
        }

        private static void PrintMatch(string filename, LineEndingType type)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("MATCH: ");
            Console.ResetColor();
            Console.WriteLine("File {0} line ending {1}", GetRelativeFilename(filename), type);
        }

        private static void LogMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        private static LineEndingType GetLineEndingType(
            string filename)
        {
            LineEndingType lt = LineEndingType.Unknown;

            bool TrySetLineEndType(LineEndingType newType)
            {
                if (lt != LineEndingType.Unknown && lt != newType)
                    return false;
                lt = newType;
                return true;
            }

            string text = File.ReadAllText(filename);
            
            int lineNo = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char Peek()
                {
                    if (i == text.Length)
                        return '\0';
                    return text[i + 1];
                }
                char ch = text[i];
                
                if (ch == '\r' && Peek() == '\n')
                {
                    if (!TrySetLineEndType(LineEndingType.CRLF))
                        return LineEndingType.Mixed;

                    // Consume the LF
                    i++;
                    lineNo++;
                }
                else if (ch == '\r')
                {
                    if (!TrySetLineEndType(LineEndingType.CR))
                        return LineEndingType.Mixed;
                    lineNo++;
                }
                else if (ch == '\n')
                {
                    if (!TrySetLineEndType(LineEndingType.LF))
                        return LineEndingType.Mixed;
                    lineNo++;
                }
            }

            if (lt == LineEndingType.Unknown)
                return LineEndingType.None;
            return lt;
        }
    }

    
    internal enum LineEndingType
    {
        Unknown,
        LF,
        CRLF,
        CR,
        Mixed,
        // File has no lines
        None,
    }
}

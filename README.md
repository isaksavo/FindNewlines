# FindNewlines
Small tool to find mixed/wrong line endings in text files

Usage: `FindNewlines.exe DIR [extensions]`

Extensions can be a list of extensions, separated by space (e.g. .cs .xaml)
If omitted, the following file types checked: `.cs .csproj .sln .xaml .xml .cpp .cxx .hpp .hxx .h .htm .html .rc`

## Examples

Check all c# files in the current directory and all subfolders
   
    FindNewlines.exe . .cs

Check all files matching the default list of extensions in c:\dev\mycode\src and subfolders
   
    FindNewlines.exe c:\dev\mycode\src

using System.IO;
using System.Reflection;

namespace AdventOfCode2018.Helpers
{
    public static class SharedFunctions
    {
        public static string GetCurrentWorkingDirectory(string appendedPath = "")
        {
            return $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\{appendedPath}";
        }
    }
}
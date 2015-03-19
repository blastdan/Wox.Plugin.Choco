using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wox.Plugin.Choco
{
    public static class Parameters
    {
        public static string ImageFilePath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + @"\Images\";
        public static string TempImageFilePath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + @"\Temp\";
        public static string FilePrefix = @"_.jpeg";
    }
}

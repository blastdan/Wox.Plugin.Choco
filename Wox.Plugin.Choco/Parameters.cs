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
#if DEBUG
        public static string ImageFilePath = Directory.GetCurrentDirectory() + @"\Images\";
#else 
        public static string ImageFilePath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + @"\Images\";
#endif
    }
}

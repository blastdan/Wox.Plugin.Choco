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
        public static string FilePrefix = @"_.jpeg";
        public static string SearchOptionTemplate = @"IsLatestVersion and ((substringof('{0}',tolower(Id)) eq true) or (substringof('{0}',tolower(Title)) eq true) or (substringof('{0}',tolower(Description)) eq true))";
        public static Uri ChocoWebApiUri = new Uri("https://chocolatey.org/api/v2/");
        public static string DefaultIconPath = Path.Combine(ImageFilePath, "icon.png");
    }
}

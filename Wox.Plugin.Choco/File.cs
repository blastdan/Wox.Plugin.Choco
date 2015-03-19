using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Choco
{
    public class FileUtilities
    {
        public static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), "_"));
        }
    }
}

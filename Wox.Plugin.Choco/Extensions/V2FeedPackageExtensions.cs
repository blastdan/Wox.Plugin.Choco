using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin.Choco.ChocoReference;
using Wox.Plugin.Choco;

namespace Wox.Plugin.Choco.Extensions
{
    public static class V2FeedPackageExtensions
    {
        public static Web.DownloadFileInformation ToDownloadFileInformation(this V2FeedPackage package)
        {
            return new Web.DownloadFileInformation(package.Id, package.IconUrl);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGetGallery;
using Wox.Plugin.Choco;

namespace NuGetGallery
{
    public static class V2FeedPackageExtensions
    {
        public static Web.DownloadFileInformation ToDownloadFileInformation(this V2FeedPackage package)
        {
            return new Web.DownloadFileInformation(package.Id, package.IconUrl);
        }
    }
}

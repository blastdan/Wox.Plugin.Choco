using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NuGetGallery;
using Wox.Plugin.Choco.Extensions;
using Wox.Plugin;

namespace Wox.Plugin.Choco
{
    public class Main : IPlugin
    {
        private PluginInitContext context { get; set; }

        public List<Result> Query(Query query)
        {
            var filter = string.Join(" ", query.ActionParameters.ToArray());
            var queryResults = Web.Query(filter);
            var downloadResults = Web.DownloadFiles(queryResults.Select(r => r.ToDownloadFileInformation()));
            var joinResults = queryResults.FullOuterJoin(downloadResults, 
                                                         q => q.Id, 
                                                         d => d.id,
                                                         (a, b, id) => new { package = a, result = b },
                                                         null, 
                                                         null)
                                           .OrderByDescending(j => j.package.DownloadCount)
                                           .ToList();
            return joinResults.Select(j => CreatePackageListItem(j.package, j.result)).ToList();
        }

        public static Result CreatePackageListItem(V2FeedPackage package, Web.DownloadFileStatus downloadStatus = null)
        {
            var icoPath = downloadStatus == null || !downloadStatus.Status ? Parameters.DefaultIconPath : downloadStatus.FilePath;

            return new Result
            {
                IcoPath = icoPath,
                Title = package.Title,
                SubTitle = string.Concat(package.Version, " - ", package.Description.Replace("\n", "")),
                ContextMenu = Lists.Of(CreateUnistallResult(package, icoPath)),
                Action = (c) =>
                {
                    Install(package.Id);
                    return true;
                }
            };
        }

        private static Result CreateUnistallResult(V2FeedPackage package, string icoPath)
        {
            return new Result
            {
                Title = "Uninstall " + package.Title,
                IcoPath = icoPath,
                Action = (c) =>
                {
                    Uninstall(package.Id);
                    return true;
                }
            };
        }

        public static void Install(string packageTitle)
        {
            var psi = new ProcessStartInfo("choco", string.Format("install {0}", packageTitle))
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(psi);
            }
            catch (Exception)
            {
                
            }
        }

        public static void Uninstall(string packageTitle)
        {
            var psi = new ProcessStartInfo("choco", string.Format("uninstall {0}", packageTitle))
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(psi);
            }
            catch (Exception)
            {

            }
        }

        public void Init(PluginInitContext context)
        {
            this.context = context;
        }

        public IEnumerable<string> ReadLines(StreamReader streamProvider)
        {
            using (var reader = streamProvider)
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}

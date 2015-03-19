using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Choco
{
    public class Main : IPlugin
    {
        private PluginInitContext context { get; set; }

        public List<Result> Query(Query query)
        {
            var filter = query.ActionParameters[0].ToLower();
            var results = Web.Query(filter);
            var iconsToDownload = results.Where(x => !string.IsNullOrEmpty(x.IconUrl)
                                                     && !x.IconUrl.EndsWith(".svg"))
                                         .Select(x => new KeyValuePair<string, string>(x.Title, x.IconUrl));

            Web.DownloadFiles(iconsToDownload);

            var final = results.Select(x => new Result
            {
                IcoPath = string.IsNullOrEmpty(x.IconUrl) ? string.Empty : Parameters.ImageFilePath + FileUtilities.CleanFileName(x.Title) + Parameters.FilePrefix,
                Title = x.Title,
                SubTitle = x.Version +  " - " + x.Description.Replace("\n", ""),
                ContextMenu = new List<Result>(){
                    new Result()
                    {
                        Title = "Uninstall",
                        IcoPath = string.IsNullOrEmpty(x.IconUrl) ? string.Empty : Parameters.ImageFilePath + FileUtilities.CleanFileName(x.Title) + Parameters.FilePrefix,
                        Action = (c) => {
                            Uninstall(x.Id);
                            return true;
                        }
                    }
                },
                Action = (c) =>
                {
                    Install(x.Id);                    
                    return true;
                }
            }).ToList();

            return final;
        }

        public void Install(string packageTitle)
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

        public void Uninstall(string packageTitle)
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
            if(!Directory.Exists(Parameters.TempImageFilePath))
            {
                Directory.CreateDirectory(Parameters.TempImageFilePath);
            }
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

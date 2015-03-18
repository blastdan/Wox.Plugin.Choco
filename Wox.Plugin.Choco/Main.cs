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
            var iconsToDownload = results.Where(x => !string.IsNullOrEmpty(x.IconUrl))
                                         .Select(x => new KeyValuePair<string, string>(x.Title, x.IconUrl));

            Web.DownloadFiles(iconsToDownload);

            var final = results.Select(x => new Result
            {
                IcoPath = Parameters.ImageFilePath + x.Title + ".jpeg",
                Title = x.Title,
                SubTitle = x.Description,
                Action = (c) =>
                {
                    Install(x.Title);
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

            Process.Start(psi);
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

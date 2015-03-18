using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Wox.Plugin.Choco.ChocoReference;

namespace Wox.Plugin.Choco
{
    public static class Web
    {
        public static IEnumerable<V2FeedPackage> Query(string criteria)
        {
            var feedClient = new FeedContext_x0060_1(new Uri("https://chocolatey.org/api/v2/"));
            var searchOptionTemplate = @"IsLatestVersion and ((substringof('{0}',tolower(Id)) eq true) or (substringof('{0}',tolower(Title)) eq true) or (substringof('{0}',tolower(Description)) eq true))";

            var query = feedClient.Packages.AddQueryOption("$filter", string.Format(searchOptionTemplate, criteria.ToLower())) as DataServiceQuery<V2FeedPackage>;
            return query.Execute().ToList();
        }

        public static void DownloadFiles(IEnumerable<KeyValuePair<string, string>> urls)
        {
            foreach (var url in urls)
            {
                if(!File.Exists(Parameters.ImageFilePath + url.Key + "jpeg"))
                {
                    var client = new WebClient();                    
                    client.DownloadFile(new Uri(url.Value), Parameters.ImageFilePath + url.Key + ".jpeg");                    
                }               
            }
        }
    }
}

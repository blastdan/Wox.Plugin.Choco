using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            var query = feedClient.Packages.AddQueryOption("$filter", string.Format(searchOptionTemplate, criteria.ToLower()));
            return query.Execute().ToList();
        }

        public static void DownloadFiles(IEnumerable<KeyValuePair<string, string>> urls)
        {
            foreach (var url in urls)
            {
                var filename = FileUtilities.CleanFileName(url.Key);
                if( !string.IsNullOrEmpty(url.Key)
                    && !File.Exists(Parameters.ImageFilePath + filename + Parameters.FilePrefix)
                    && !File.Exists(Parameters.TempImageFilePath + filename + Parameters.FilePrefix))
                {
                    var client = new WebClient();
                    var downloader = new Downloader(client, url.Value, url.Key);
                    downloader.Download();                 
                }               
            }
        }

        private class Downloader : IDisposable
        {
            private readonly Uri downloadUrl;
            private readonly string fileName;
            private readonly WebClient client;

            private bool disposed;

            public Downloader(WebClient client, string downloadUrl, string fileName)
            {
                this.downloadUrl = new Uri(downloadUrl);
                this.fileName = fileName + Parameters.FilePrefix;
                this.client = client;
                this.client.DownloadFileCompleted += client_DownloadFileCompleted;
            }            

            public void Download()
            {
                client.DownloadFileAsync(this.downloadUrl, this.TempFilePath);
            }

            private string TempFilePath
            {
                get
                {
                    return Path.Combine(Parameters.TempImageFilePath, FileUtilities.CleanFileName(this.fileName));
                }
            }

            private string FilePath
            {
                get
                {
                    return Path.Combine(Parameters.ImageFilePath, FileUtilities.CleanFileName(this.fileName));
                }
            }

            private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
            {
                var client = sender as WebClient;
                var fileInfo = new FileInfo(this.TempFilePath);

                if(fileInfo.Length < 100)
                {
                    File.Delete(this.TempFilePath);
                }
                else if(!e.Cancelled || e.Error != null)
                {
                    File.Move(this.TempFilePath, this.FilePath);
                }              
                
                this.Dispose();
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            // Protected implementation of Dispose pattern. 
            protected virtual void Dispose(bool disposing)
            {
                if (disposed)
                    return;

                if (disposing)
                {
                    this.client.Dispose();
                }

                disposed = true;
            }
        }
    }
}

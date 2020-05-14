using NuGetGallery;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using NuGetGallery;

namespace Wox.Plugin.Choco
{
    public static class Web
    {
        public class DownloadFileInformation
        {
            private readonly string futureFileName;
            public readonly string id;
            public readonly string url;            

            public DownloadFileInformation(string id, string url)
            {
                this.id = id;
                this.url = url;
                this.futureFileName = string.Concat(id, Parameters.FilePrefix);
            }

            public string FilePath
            {
                get
                {
                    return Path.Combine(Parameters.ImageFilePath, FileUtilities.CleanFileName(this.futureFileName));
                }
            }

            public DownloadFileStatus FileExists()
            {
                return File.Exists(FilePath) 
                    ? new DownloadFileStatus(this, true) 
                    : default(DownloadFileStatus);
            }

            /// <summary>
            /// Basic checks for empty strings,
            /// Wox can't handle SVG's
            /// Make sure the doesn't already exist
            /// </summary>
            /// <returns></returns>
            public bool CanBeDownloaded()
            {
                return !string.IsNullOrEmpty(this.id)
                    && !string.IsNullOrEmpty(this.url)
                    && !this.url.EndsWith(".svg");       
            }

            public Uri Uri { get { return new Uri(this.url); } }

            public override int GetHashCode()
            {
                return this.id.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var b = obj as DownloadFileInformation;

                return obj == null || b == null
                       ? false
                       : this.id == b.id;
            }
        }

        public class DownloadFileStatus : DownloadFileInformation
        {
            private readonly bool result;

            public DownloadFileStatus (string id, string url, bool result) 
                : base(id, url)
	        {
                this.result = result;
	        }

            public DownloadFileStatus(DownloadFileInformation information, bool result)
                : this(information.id, information.url, result)
            {
            }

            public bool Status { get { return this.result; } }
        }

        /// <summary>
        /// Search packages using the Chocolatey oData feed
        /// </summary>
        /// <param name="criteria"> The term to serach for</param>
        /// <returns></returns>
        public static IEnumerable<V2FeedPackage> Query(string criteria)
        {
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var feedClient = new FeedContext_x0060_1(Parameters.ChocoWebApiUri);
            var query = feedClient.Packages.AddQueryOption("$filter", string.Format(Parameters.SearchOptionTemplate, criteria.ToLower()));
            return query.Execute().ToList();
        }

        public static IEnumerable<DownloadFileStatus> DownloadFiles(IEnumerable<DownloadFileInformation> fileDescriptions)
        {
            var existingFiles = fileDescriptions.Where(f => f.CanBeDownloaded())
                                                .Select(f => f.FileExists())
                                                .Where(f => f != null)
                                                .ToList();
            return fileDescriptions.Where(f => f.CanBeDownloaded())
                                   .Except(existingFiles.Cast<DownloadFileInformation>())
                                   .Select(f => Downloader.Run(f))
                                   .Select(d => d.Wait())
                                   .Select(d => d.DownloadFileStatus)
                                   .Union(existingFiles);
        }

        private class Downloader : IDisposable
        {
            private readonly DownloadFileInformation information;
            private readonly WebClient client;
            private readonly EventWaitHandle waitHandle = new AutoResetEvent(false);
            private bool downloadSuccessful = false;
            private bool disposed;

            public bool downloadStarted = false;
            public static ConcurrentDictionary<string, Downloader> CurrentlyDownloading = new ConcurrentDictionary<string, Downloader>();


            private Downloader(WebClient client, DownloadFileInformation information)
            {
                this.information = information;
                this.client = client;
                this.client.DownloadFileCompleted += client_DownloadFileCompleted;
            }

            public DownloadFileStatus DownloadFileStatus
            {
                get
                {
                    return new DownloadFileStatus(this.information, this.downloadSuccessful);                    
                }
            }

            public static Downloader Run(DownloadFileInformation information)
            {
                var client = new WebClient();
                var downloader = new Downloader(client, information);
                downloader = Downloader.CurrentlyDownloading.GetOrAdd(information.FilePath, downloader);
                
                if(!downloader.downloadStarted)
                {
                    downloader.downloadStarted = true;
                    downloader.Download();
                }
               
                return downloader;
            }

            public Downloader Wait()
            {
                this.waitHandle.WaitOne(new TimeSpan(0, 0, 30));
                var me = this;
                Downloader.CurrentlyDownloading.TryRemove(this.DownloadFileStatus.FilePath, out me);
                return this;
            }

            private void Download()
            {
                client.DownloadFileAsync(this.information.Uri, this.information.FilePath);
            }

            private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
            {
                var fileInfo = new FileInfo(this.information.FilePath);

                // Ensure no 0 byte files
                if(fileInfo.Length < 100)
                {
                    try
                    {
                        File.Delete(this.information.FilePath);
                    }
                    catch
                    {
                        // Race conditions are causing the file to not be deleted at times.
                        // I don't really know how to stop this condition right now.
                    }
                    finally 
                    {
                        this.downloadSuccessful = false;
                    }                    
                }
                else if(e.Cancelled || e.Error != null)
                {
                    this.downloadSuccessful = this.information.FileExists() != null;
                }
                else
                {
                    this.downloadSuccessful = true;
                }

                this.waitHandle.Set();
                this.Dispose();
            }

            #region IDisposable
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
            #endregion
        }
    }
}

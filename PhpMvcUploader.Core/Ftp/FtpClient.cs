﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using PhpMvcUploader.Common;

namespace PhpMvcUploader.Core.Ftp
{
    public class FtpClient
    {
        private readonly Uri _url;

        public FtpClient(string url)
        {
            _url = new Uri(url);
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public IEnumerable<string> ListFilesRecursive(string folder = "")
        {
            var infos = GetTextResponse(WebRequestMethods.Ftp.ListDirectoryDetails, GetUri(folder));
            var folders = infos.Where(IsFolder).Select(f => GetName(f, folder));
            var files = infos.Where(i => !IsFolder(i)).Select(f => GetName(f, folder));
            return files.Union(folders.SelectMany(ListFilesRecursive));
        }

        public IEnumerable<string> ListFoldersRecursive(string folder = "")
        {
            var infos = GetTextResponse(WebRequestMethods.Ftp.ListDirectoryDetails, GetUri(folder));
            var folders = infos.Where(IsFolder).Select(f => GetName(f, folder)).ToList();
            return folders.Union(folders.SelectMany(ListFoldersRecursive));
        } 

        public void DeleteFile(string filePath)
        {
            GetTextResponse(WebRequestMethods.Ftp.DeleteFile, GetUri(filePath));
        }

        public void Download(string serverPath, string localPath)
        {
            const string method = WebRequestMethods.Ftp.DownloadFile;
            var uri = GetUri(serverPath);
            var request = CreateRequest(uri, method);

            using (var stream = File.Create(localPath))
            using (var response = GetResponse(request))
            using (var inStream = response.GetResponseStream())
            {
                ProxyBytes(inStream, stream);
            }
        }

        public void DownloadAll(string localPath)
        {
            ListFoldersRecursive()
                .OrderBy(p => p)
                .Select(p => p.Trim('/', '\\'))
                .Select(p => Path.Combine(localPath, p))
                .ForEach(p => Directory.CreateDirectory(p));
            ListFilesRecursive()
                .OrderBy(p => p)
                .Select(p => p.Trim('/', '\\'))
                .ForEach(p => Download(p, Path.Combine(localPath, p)));
        }

        private void ProxyBytes(Stream responseStream, Stream targetStream)
        {
            responseStream.NullGuard();
            var buffer = new byte[2048];
            int bytesRead;
            while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                targetStream.Write(buffer, 0, bytesRead);
            }
        }

        private IList<string> ReadTextResponse(Stream responseStream)
        {
            responseStream.NullGuard();
            var response = new List<string>();
            using (var reader = GetReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    response.Add(reader.ReadLine());
                }
            }
            return response;
        }

        private Uri GetUri(string path)
        {
            return new Uri(_url, path);
        }

        private string GetName(string folderInfo, string rootFolder)
        {
            return "{0}/{1}".FormatX(rootFolder, GetName(folderInfo));
        }

        private string GetName(string folderInfo)
        {
            return folderInfo.Substring(49);
        }

        private bool IsFolder(string folderInfo)
        {
            return folderInfo.StartsWith("d");
        }

        private IList<string> GetTextResponse(string method, Uri url = null)
        {
            if (url == null)
            {
                url = _url;
            }
            var request = CreateRequest(url, method);

            using (var response = GetResponse(request))
            using (var responseStream = response.GetResponseStream())
            {
                return ReadTextResponse(responseStream);
            }
        }

        private WebRequest CreateRequest(Uri url, string method)
        {
            WebRequest request;
            try
            {
                request = WebRequest.Create(url);
            }
            catch (UriFormatException ex)
            {
                throw new UriFormatException(url.AbsoluteUri, ex);
            }
            request.Method = method;
            if (Username != null && Password != null)
            {
                request.Credentials = new NetworkCredential(Username, Password);
            }
            return request;
        }

        private StreamReader GetReader(Stream responseStream)
        {
            try
            {
                return new StreamReader(responseStream);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(responseStream.ToString(), ex);
            }
        }

        private WebResponse GetResponse(WebRequest request)
        {
            try
            {
                return request.GetResponse();
            }
            catch (WebException ex)
            {
                throw new WebException(request.RequestUri.AbsoluteUri, ex);
            }
        }
    }
}

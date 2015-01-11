using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using NUnit.Framework;
using PhpMvcUploader.Common;
using PhpMvcUploader.Core.Ftp;
using PhpMvcUploader.Core.Test.TestConfig;
using PhpMvcUploader.Test.Helpers.TestData;

namespace PhpMvcUploader.Core.Test.Ftp
{
    [TestFixture]
    public class FtpClientTest
    {
        private bool _doRun;
        private string _localDirectory;
        private string _username;
        private string _password;
        private string _url;
        private Generator _generator;

        private const int MaxFolderDepth = 5;
        private const int MaxFilesPerFolder = 4;
        private const int MaxFoldersPerFolder = 3;

        private FtpClient _client;
        private List<string> _files;
        private List<string> _folders;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            var config = new AppConfig();
            _doRun = config.RunFtpTests;
            if (!_doRun) return;

            _localDirectory = config.FtpLocalPath;
            _username = config.FtpUsername;
            _password = config.FtpPassword;
            _url = config.FtpUrl;

            _generator = new Generator();
            _client = new FtpClient(_url)
            {
                Username = _username,
                Password = _password
            };
            SetUpLocalFiles();
        }

        private void SetUpLocalFiles()
        {
            foreach(var d in Directory.EnumerateDirectories(_localDirectory))
            {
                Directory.Delete(d, true);
            }
            foreach (var f in Directory.EnumerateFiles(_localDirectory))
            {
                File.Delete(f);
            }
            _files = new List<string>();
            _folders = new List<string>();
            MakeFolderTree();
        }

        private void MakeFolderTree(string start = null, int depth = MaxFolderDepth)
        {
            if (start == null)
            {
                start = _localDirectory;
            }
            var numberOfFiles = _generator.RandomIntInclusive(1, MaxFilesPerFolder);
            for (var i = 0; i < numberOfFiles; i++)
            {
                var newFileName = _generator.GetUniqueString();
                var newFilePath = Path.Combine(start, newFileName);
                _files.Add(newFilePath);
                var stream = File.Create(newFilePath);
                stream.Close();
            }
            if (depth == 0)
            {
                return;
            }
            var numberOfFolders = _generator.RandomIntInclusive(1, MaxFoldersPerFolder);
            for (var i = 0; i < numberOfFolders; i++)
            {
                var newFolder = Path.Combine(start, _generator.GetUniqueString());
                _folders.Add(newFolder);
                Directory.CreateDirectory(newFolder);
                MakeFolderTree(newFolder, depth - 1);
            }
        }

        [Test]
        public void ListFilesRecursiveWorks()
        {
            if (!_doRun) return;
            var expected = Directory
                .EnumerateFiles(_localDirectory, "*", SearchOption.AllDirectories);

            var list = _client.ListFilesRecursive();

            AssertEquivalent(list, expected);
        }

        [Test]
        public void DeleteFileWorks()
        {
            var fileToDelete = _generator.RandomElement(_files);
            var relativeFilePath = StripLocalDirectory(fileToDelete);
            Console.WriteLine(relativeFilePath);

            _client.DeleteFile(relativeFilePath);

            Assert.That(File.Exists(fileToDelete), Is.False);
        }

        private void AssertEquivalent(IEnumerable<string> actual, IEnumerable<string> expected)
        {
            var sanitized = expected.Select(StripLocalDirectory);

            Assert.That(actual, Is.EquivalentTo(sanitized));
        }

        private string StripLocalDirectory(string actual)
        {
            var stripped = actual.StartsWith(_localDirectory)
                ? actual.Substring(_localDirectory.Length)
                : actual;
            var forwardSlashes = stripped.Replace('\\', '/').Trim('/');
            return "/{0}".FormatX(forwardSlashes);
        }
    }
}

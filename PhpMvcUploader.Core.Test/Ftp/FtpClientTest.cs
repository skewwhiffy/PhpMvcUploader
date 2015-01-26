using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private const int IoRetry = 15;
        private static readonly TimeSpan IoRetryInterval = TimeSpan.FromMilliseconds(100);

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
        private string _workspace;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            var config = new AppConfig();
            _doRun = config.RunFtpTests;
            if (!_doRun)
            {
                return;
            }

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

        [TestFixtureTearDown]
        public void AfterAll()
        {
            DeleteLocalFiles();
            DeleteWorkspace();
        }

        private void SetUpLocalFiles()
        {
            DeleteLocalFiles();
            _files = new List<string>();
            _folders = new List<string>();
            MakeFolderTree();
            SetupWorkspace();
        }

        private void DeleteLocalFiles()
        {
            if (_localDirectory.IsNullOrEmpty() || !Directory.Exists(_localDirectory))
            {
                return;
            }
            foreach (var d in Directory.EnumerateDirectories(_localDirectory))
            {
                DeleteFolder(d);
            }
            foreach (var f in Directory.EnumerateFiles(_localDirectory))
            {
                var file = f;
                IoRetry.TimesEvery(IoRetryInterval).Try(() => File.Delete(file));
            }
        }

        private void SetupWorkspace()
        {
            _workspace = Path.Combine(Environment.CurrentDirectory, "workspace");
            DeleteWorkspace();
            Directory.CreateDirectory(_workspace);
        }

        private void DeleteWorkspace()
        {
            if (_workspace.IsNullOrEmpty() || !Directory.Exists(_workspace))
            {
                return;
            }
            DeleteFolder(_workspace);
        }

        private void DeleteFolder(string folder)
        {
            try
            {
                IoRetry.TimesEvery(IoRetryInterval).Try(() => Directory.Delete(folder, true));
            }
            catch
            {
                // Do nothing: this just leaves a few files behind.
            }
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
                var newFilePath = Path.Combine(start, "{0}.txt".FormatX(newFileName));
                var newFileContents = "This is file {0}{1}"
                    .FormatX(newFilePath, Environment.NewLine)
                    .GetBytes();
                _files.Add(newFilePath);
                using (var stream = File.Create(newFilePath))
                {
                    stream.Write(newFileContents, 0, newFileContents.Length);
                    stream.Close();
                }
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
        public void ListFoldersRecursiveWorks()
        {
            if (!_doRun) return;
            var expected = Directory
                .EnumerateDirectories(_localDirectory, "*", SearchOption.AllDirectories);

            var list = _client.ListFoldersRecursive();

            AssertEquivalent(list, expected);
        }

        [Test]
        public void DeleteFileWorks()
        {
            if (!_doRun) return;
            var fileToDelete = _generator.RandomElement(_files);
            var relativeFilePath = StripLocalDirectory(fileToDelete);
            Console.WriteLine(relativeFilePath);

            _client.DeleteFile(relativeFilePath);

            Assert.That(File.Exists(fileToDelete), Is.False);
        }

        [Test]
        public void DownloadFileWorks()
        {
            if (!_doRun) return;
            var target = Path.Combine(_workspace, "blah.txt");
            var source = _files.Random();
            var sourceRelativePath = StripLocalDirectory(source);
            var sourceContents = File.OpenText(source).ReadToEnd();

            _client.Download(sourceRelativePath, target);

            Assert.That(File.Exists(target));
            var downloadedContents = File.OpenText(target).ReadToEnd();
            Assert.That(downloadedContents, Is.EqualTo(sourceContents));
        }

        [Test]
        public void DownloadAllWorks()
        {
            if (!_doRun) return;
            var target = Path.Combine(_workspace, "temp");
            Directory.CreateDirectory(target);
            var targetFilesExpected = _files
                .Select(GetRelativePath)
                .Select(p => Path.Combine(target, p));
            var targetFoldersExpected = _folders
                .Select(GetRelativePath)
                .Select(p => Path.Combine(target, p));

            _client.DownloadAll(target);

            targetFilesExpected.ForEach(f => Assert.That(File.Exists(f), f));
            targetFoldersExpected.ForEach(f => Assert.That(Directory.Exists(f), f));
        }

        private void AssertEquivalent(IEnumerable<string> actual, IEnumerable<string> expected)
        {
            var sanitized = expected.Select(StripLocalDirectory);

            Assert.That(actual, Is.EquivalentTo(sanitized));
        }

        private string GetRelativePath(string actual)
        {
            var stripped = actual.StartsWith(_localDirectory)
                ? actual.Substring(_localDirectory.Length)
                : actual;
            return stripped.Replace('\\', '/').Trim('/');
        }

        private string StripLocalDirectory(string actual)
        {
            return "/{0}".FormatX(GetRelativePath(actual));
        }
    }
}

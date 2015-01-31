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
        private bool _doRun;
        private string _localDirectory;
        private string _username;
        private string _password;
        private string _url;
        private Generator _generator;
        private FileMaker _fileMaker;

        private FtpClient _client;
        private List<string> _files;
        private List<string> _folders;
        private string _workspace;

        [SetUp]
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

        [TearDown]
        public void AfterAll()
        {
            DeleteLocalFiles();
            DeleteWorkspace();
        }

        private void SetUpLocalFiles()
        {
            DeleteLocalFiles();
            _fileMaker = FileMaker.AbsolutePaths(_localDirectory);
            _files = _fileMaker.Files;
            _folders = _fileMaker.Folders;
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
                _fileMaker.DeleteFolder(d);
            }
            foreach (var f in Directory.EnumerateFiles(_localDirectory))
            {
                _fileMaker.DeleteFile(f);
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
            _fileMaker.DeleteFolder(_workspace);
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

            _client.DeleteFile(relativeFilePath);

            Assert.That(File.Exists(fileToDelete), Is.False);
            Console.WriteLine(fileToDelete);
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

            var missing = targetFilesExpected
                .Where(f => !File.Exists(f))
                .Union(targetFoldersExpected
                    .Where(f => !Directory.Exists(f)))
                .JoinX();
            Assert.That(missing, Is.Empty);
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

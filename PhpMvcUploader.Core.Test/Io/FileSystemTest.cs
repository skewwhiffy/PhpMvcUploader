using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PhpMvcUploader.Core.Io;
using PhpMvcUploader.Test.Helpers.TestData;

namespace PhpMvcUploader.Core.Test.Io
{
    [TestFixture]
    public class FileSystemTest
    {
        private string _localDirectory;
        private FileMaker _fileMaker;

        private FileSystem _fileSystem;

        [SetUp]
        public void BeforeEach()
        {
            _localDirectory = Path.Combine(Environment.CurrentDirectory, "Workspace");
            FileMaker.DeleteFolderAbsolute(_localDirectory);
            Directory.CreateDirectory(_localDirectory);
            _fileMaker = FileMaker.RelativePaths(_localDirectory);
            _fileSystem = new FileSystem(_localDirectory);
        }

        [TearDown]
        public void AfterEach()
        {
            FileMaker.DeleteFolderAbsolute(_localDirectory);
        }

        [Test]
        public void GetFilesRecursiveWorksAtRoot()
        {
            Assert.That(_fileMaker.Files, Is.EquivalentTo(_fileSystem.GetFilesRecursive()));
        }

        [Test]
        public void GetAlsolutePath()
        {
            Assert.That(_fileMaker.Files.Select(f => _fileSystem.AbsolutePath(f)).All(File.Exists));
        }
    }
}

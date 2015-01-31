using System;
using System.IO;
using NUnit.Framework;
using PhpMvcUploader.Common;
using PhpMvcUploader.Core.Comparison;
using PhpMvcUploader.Core.Io;
using PhpMvcUploader.Test.Helpers.TestData;

namespace PhpMvcUploader.Core.Test.Comparison
{
    [TestFixture]
    public class FolderComparerTest
    {
        private string _beforeFolder;
        private string _afterFolder;
        private FileMaker _fileMaker;
        private FolderComparer _comparer;
        private StreamComparer _fileComparer;
        private Generator _generator;

        [SetUp]
        public void BeforeEach()
        {
            _beforeFolder = Path.Combine(Environment.CurrentDirectory, "beforeWorkspace");
            _afterFolder = Path.Combine(Environment.CurrentDirectory, "afterWorkspace");
            FileMaker.DeleteFolderAbsolute(_beforeFolder);
            FileMaker.DeleteFolderAbsolute(_afterFolder);
            Directory.CreateDirectory(_beforeFolder);
            Directory.CreateDirectory(_afterFolder);
            _fileMaker = FileMaker.RelativePaths(_beforeFolder);
            FileMaker.CopyFolderAbsolute(_beforeFolder, _afterFolder);
            _comparer = null;
            _generator = new Generator();
            _fileComparer = new StreamComparer();
        }

        [TearDown]
        public void AfterEach()
        {
            _fileMaker.DeleteFolder(_beforeFolder);
            _fileMaker.DeleteFolder(_afterFolder);
        }

        private FolderComparer Comparer
        {
            get { return _comparer ?? (_comparer = new FolderComparer(new FileSystemFactory(), _fileComparer, _beforeFolder, _afterFolder)); }
        }

        [Test]
        public void IdenticalFoldersReturnNothing()
        {
            Assert.That(Comparer.Different, Is.Empty);
            Assert.That(Comparer.DeletedFiles, Is.Empty);
            Assert.That(Comparer.DeletedFolders, Is.Empty);
            Assert.That(Comparer.NewFiles, Is.Empty);
            Assert.That(Comparer.NewFolders, Is.Empty);
        }

        [Test]
        public void AddedDirectoryGetsReturned()
        {
            var randomFolder = _fileMaker.Folders.Random();
            var newFolder = Path.Combine(randomFolder, _generator.GetUniqueString());
            var newAfterFolder = Path.Combine(_afterFolder, newFolder);
            Directory.CreateDirectory(newAfterFolder);

            Assert.That(Comparer.DeletedFolders, Is.Empty);
            Assert.That(Comparer.DeletedFiles, Is.Empty);
            Assert.That(Comparer.NewFiles, Is.Empty);
            Assert.That(Comparer.Different, Is.Empty);
            Assert.That(Comparer.NewFolders.Count, Is.EqualTo(1));
            Assert.That(Comparer.NewFolders[0], Is.EqualTo(newFolder));
        }

        [Test]
        public void AddedFileInNewDirectoryGetsReturned()
        {
            var randomFolder = _fileMaker.Folders.Random();
            var newFolder = Path.Combine(randomFolder, _generator.GetUniqueString());
            var newAfterFolder = Path.Combine(_afterFolder, newFolder);
            Directory.CreateDirectory(newAfterFolder);
            var newFile = Path.Combine(newFolder, "{0}.txt".FormatX(_generator.GetUniqueString()));
            using (var newFileHandle = File.CreateText(Path.Combine(_afterFolder, newFile)))
            {
                newFileHandle.Write("This is a brand new file");
            }

            Assert.That(Comparer.DeletedFolders, Is.Empty);
            Assert.That(Comparer.DeletedFiles, Is.Empty);
            Assert.That(Comparer.Different, Is.Empty);
            Assert.That(Comparer.NewFolders.Count, Is.EqualTo(1));
            Assert.That(Comparer.NewFolders[0], Is.EqualTo(newFolder));
            Assert.That(Comparer.NewFiles.Count, Is.EqualTo(1));
            Assert.That(Comparer.NewFiles[0], Is.EqualTo(newFile));
        }

        [Test]
        public void AddedFileInExistingDirectoryGetsReturned()
        {
            var randomFolder = _fileMaker.Folders.Random();
            var newFile = Path.Combine(randomFolder, "{0}.txt".FormatX(_generator.GetUniqueString()));
            using (var newFileHandle = File.CreateText(Path.Combine(_afterFolder, newFile)))
            {
                newFileHandle.Write("This is a brand new file");
            }

            Assert.That(Comparer.DeletedFolders, Is.Empty);
            Assert.That(Comparer.DeletedFiles, Is.Empty);
            Assert.That(Comparer.Different, Is.Empty);
            Assert.That(Comparer.NewFolders, Is.Empty);
            Assert.That(Comparer.NewFiles.Count, Is.EqualTo(1));
            Assert.That(Comparer.NewFiles[0], Is.EqualTo(newFile));
        }

        [Test]
        public void DeletedDirectoryGetsReturned()
        {
            var randomFolder = _fileMaker.Folders.Random();
            FileMaker.DeleteFolderAbsolute(Path.Combine(_afterFolder, randomFolder));

            Assert.That(Comparer.NewFolders, Is.Empty);
            Assert.That(Comparer.DeletedFiles, Is.Empty);
            Assert.That(Comparer.NewFiles, Is.Empty);
            Assert.That(Comparer.Different, Is.Empty);
            Assert.That(Comparer.DeletedFolders.Count, Is.EqualTo(1));
            Assert.That(Comparer.DeletedFolders[0], Is.EqualTo(randomFolder));
        }

        [Test]
        public void ChangedFileGetsReturned()
        {
            var randomFile = _fileMaker.Files.Random();
            var afterFile = Path.Combine(_afterFolder, randomFile);
            using (var fileHandle = File.OpenWrite(afterFile))
            {
                var bytes = "changed".GetBytes();
                fileHandle.Write(bytes, 0, bytes.Length);
            }

            Assert.That(Comparer.DeletedFolders, Is.Empty);
            Assert.That(Comparer.DeletedFiles, Is.Empty);
            Assert.That(Comparer.NewFiles, Is.Empty);
            Assert.That(Comparer.NewFolders, Is.Empty);
            Assert.That(Comparer.Different.Count, Is.EqualTo(1));
            Assert.That(Comparer.Different[0], Is.EqualTo(randomFile));
        }
    }
}

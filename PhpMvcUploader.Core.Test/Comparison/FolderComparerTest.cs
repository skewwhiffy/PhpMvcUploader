using System;
using System.IO;
using NUnit.Framework;
using PhpMvcUploader.Test.Helpers.TestData;

namespace PhpMvcUploader.Core.Test.Comparison
{
    [TestFixture]
    public class FolderComparerTest
    {
        private string _beforeFolder;
        private string _afterFolder;
        private FileMaker _fileMaker;

        [SetUp]
        public void BeforeEach()
        {
            _beforeFolder = Path.Combine(Environment.CurrentDirectory, "beforeWorkspace");
            _afterFolder = Path.Combine(Environment.CurrentDirectory, "afterWorkspace");
            _fileMaker = new FileMaker();
            _fileMaker.DeleteFolder(_beforeFolder);
            _fileMaker.DeleteFolder(_afterFolder);
            Directory.CreateDirectory(_beforeFolder);
            Directory.CreateDirectory(_afterFolder);
            _fileMaker.MakeFolderTree(_beforeFolder);
            _fileMaker.CopyFolder(_beforeFolder, _afterFolder);
        }

        [TearDown]
        public void AfterEach()
        {
            _fileMaker.DeleteFolder(_beforeFolder);
            _fileMaker.DeleteFolder(_afterFolder);
        }

        [Test]
        public void DoesItWork()
        {
            Console.WriteLine(_beforeFolder);
            Console.WriteLine(_afterFolder);
        }
    }
}

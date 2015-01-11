using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PhpMvcUploader.Core.Execute;

namespace PhpMvcUploader.Core.Test.Execute
{
    [TestFixture]
    public class ExecuteTest
    {
        private string _echoArgsPath;
        private Executable _exe;

        [SetUp]
        public void BeforeEach()
        {
            _echoArgsPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "resources\\echoargs.exe");

            _exe = new Executable(_echoArgsPath);
        }

        [Test]
        public void ExecuteDoesNotThrow()
        {
            Assert.DoesNotThrow(() => _exe.Execute());
        }

        [Test]
        public void ExecuteRetrievesOutput()
        {
            var args = new[] {"arg1", "arg2"};

            var output = _exe.Execute(args);

            Assert.IsNotNull(output);
            Assert.That(args.All(a => output.Contains(a)), string.Join(Environment.NewLine, output));
        }
    }
}

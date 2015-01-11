using System.Collections.Generic;
using System.Diagnostics;
using PhpMvcUploader.Common;

namespace PhpMvcUploader.Core.Execute
{
    public class Executable
    {
        private readonly string _path;

        public Executable(string path)
        {
            _path = path;
        }

        public List<string> Execute(params string[] args)
        {
            var info = GetStartInfo(args);
            var retMessage = new List<string>();
            using (var process = Process.Start(info))
            {
                if (process == null)
                {
                    throw new ExecutableException();
                }
                while (!process.StandardOutput.EndOfStream)
                {
                    retMessage.Add(process.StandardOutput.ReadLine());
                }
            }
            return retMessage;
        }

        private ProcessStartInfo GetStartInfo(string[] args)
        {
            return new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = _path,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                Arguments = args.JoinX(" ")
            };
        }
    }
}

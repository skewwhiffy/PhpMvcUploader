using System;

namespace PhpMvcUploader.Core.Settings
{
    [Serializable]
    public class Config : IConfig
    {
        public Config()
        {
            GitExecutable = "C:\\dev\\Git\\bin\\git.exe";
        }

        public string GitExecutable { get; set; } 
    }
}

using PhpMvcUploader.Test.Helpers;
using PhpMvcUploader.Test.Helpers.Config;

namespace PhpMvcUploader.Core.Test.TestConfig
{
    public class AppConfig : AppConfigReader
    {
        public string FtpUrl
        {
            get { return GetString("FtpUrl"); }
        }

        public string FtpUsername
        {
            get { return GetString("FtpUsername"); }
        }

        public string FtpPassword
        {
            get { return GetString("FtpPassword"); }
        }

        public bool RunFtpTests
        {
            get { return GetBool("RunFtpTests"); }
        }

        public string FtpLocalPath
        {
            get { return GetString("FtpLocalPath"); }
        }
    }
}

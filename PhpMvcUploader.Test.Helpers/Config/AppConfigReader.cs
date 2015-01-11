using System.Configuration;
using PhpMvcUploader.Common;

namespace PhpMvcUploader.Test.Helpers.Config
{
    public abstract class AppConfigReader
    {
        public bool GetBool(string key, bool? defaultValue = null)
        {
            var stringValue = GetString(key) ?? string.Empty;
            switch (stringValue.ToLowerInvariant())
            {
                case "true":
                case "t":
                case "1":
                    return true;
                case "false":
                case "f":
                case "0":
                    return false;
                default:
                    if (defaultValue.HasValue)
                    {
                        return defaultValue.Value;
                    }
                    throw new ConfigurationErrorsException("Expected bool, but got {0}".FormatX(stringValue));
            }
        }

        public string GetString(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}

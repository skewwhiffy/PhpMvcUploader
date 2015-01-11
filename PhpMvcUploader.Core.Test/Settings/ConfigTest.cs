using System;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using PhpMvcUploader.Core.Settings;

namespace PhpMvcUploader.Core.Test.Settings
{
    [TestFixture]
    public class SettingsTest
    {
        [Test]
        public void GenerateXmlSerialization()
        {
            var settings = new Config();
            var textWriter = new StringWriter();

            new XmlSerializer(settings.GetType()).Serialize(textWriter, settings);
            var serialized = textWriter.ToString();

            Console.WriteLine(serialized);
        }
    }
}

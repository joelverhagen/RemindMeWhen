using System;
using System.Configuration;
using System.IO;
using FluentAssertions;
using Knapcode.RemindMeWhen.Core.Settings;
using Knapcode.RemindMeWhen.Core.Tests.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Knapcode.RemindMeWhen.Core.Tests.Settings
{
    [TestClass]
    public class RemindMeWhenConfigurationTests
    {
        [TestMethod]
        public void Constructor_FullConfiguration_Works()
        {
            // ARRANGE
            var expected = new RemindMeWhenSettings
            {
                AzureStorage = new AzureStorageSettings
                {
                    ConnectionString = GetRandomString(),
                    ProcessDocumentQueueName = GetRandomString(),
                    DocumentBlobContainerName = GetRandomString(),
                    DocumentMetadataTableName = GetRandomString()
                },
                RottenTomatoes = new RottenTomatoesSettings
                {
                    ApiKey = GetRandomString()
                }
            };

            using (var configurationFile = new ConfigurationFile(expected))
            {
                // ACT
                RemindMeWhenConfiguration configuration = configurationFile.GetConfiguration();

                // ASSERT
                RemindMeWhenSettings actual = configuration.GetSettings();
                actual.ShouldBeEquivalentTo(expected);

                configurationFile.Delete = true;
            }
        }

        private static string GetRandomString()
        {
            return Guid.NewGuid().ToString();
        }

        private class ConfigurationFile : IDisposable
        {
            private const string ConfigurationFileFormat = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
    <configSections>
        <section name=""{SectionName}"" type=""{AssemblyQualifiedName}"" />
    </configSections>
    <{SectionName}>
        <azureStorage connectionString=""{Settings.AzureStorage.ConnectionString}""
                      documentMetadataTableName=""{Settings.AzureStorage.DocumentMetadataTableName}""
                      documentBlobContainerName=""{Settings.AzureStorage.DocumentBlobContainerName}""
                      processDocumentQueueName=""{Settings.AzureStorage.ProcessDocumentQueueName}"" />

        <rottenTomatoes apiKey=""{Settings.RottenTomatoes.ApiKey}"" />
    </{SectionName}>
</configuration>
";

            private readonly string _fileName;

            public ConfigurationFile(RemindMeWhenSettings settings)
            {
                // generate the file content
                string configurationFile = ConfigurationFileFormat.FormatWith(new
                {
                    RemindMeWhenConfiguration.SectionName,
                    typeof (RemindMeWhenConfiguration).AssemblyQualifiedName,
                    Settings = settings
                });

                // write the file
                _fileName = Path.GetRandomFileName();
                File.WriteAllText(_fileName, configurationFile.Trim());
            }

            public bool Delete { get; set; }

            public void Dispose()
            {
                if (Delete)
                {
                    try
                    {
                        File.Delete(_fileName);
                    }
                    catch (IOException)
                    {
                    }
                }
            }

            public RemindMeWhenConfiguration GetConfiguration()
            {
                var map = new ExeConfigurationFileMap {ExeConfigFilename = _fileName};
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                return configuration.Sections[RemindMeWhenConfiguration.SectionName] as RemindMeWhenConfiguration;
            }
        }
    }
}
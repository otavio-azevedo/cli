using Cmf.CLI.Constants;
using Cmf.CLI.Core.Interfaces;
using Cmf.CLI.Core.Objects;
using Cmf.CLI.Factories;
using Cmf.CLI.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace tests.Specs;

public class Bump
{
    [Theory]
    [InlineData("'", "1.0.0")]
    [InlineData("\"", "1.0.0")]
    [InlineData("'", "")]
    public void Bump_MetadataWithAnyQuoteType(string quoteType, string version)
    {
        // files
        string cmfPackageJson = $"help/{CliConstants.CmfPackageFileName}";
        string npmPackageJson = "/help/package.json";
        string metadataTS = "/help/src/packages/cmf.docs.area.cmf.custom.help/src/cmf.docs.area.cmf.custom.help.metadata.ts";

        string bumpVersion = "1.0.1";

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { MockUnixSupport.Path(@"c:\.project-config.json"), new MockFileData(
                @"{
              ""MESVersion"": ""9.0.0""
            }")
            },
            { cmfPackageJson, new MockFileData(
                @$"{{
                      ""packageId"": ""Cmf.Custom.Help"",
                      ""version"": ""{version}"",
                      ""description"": ""Cmf Custom Cmf.Custom.Help Package"",
                      ""packageType"": ""Help"",
                      ""isInstallable"": true,
                      ""isUniqueInstall"": false,
                      ""contentToPack"": [
                        {{
                          ""source"": ""src/packages/*"",
                          ""target"": ""node_modules"",
                          ""ignoreFiles"": [
                            "".npmignore""
                          ]
                        }}
                      ]
                }}")
            },
            { npmPackageJson, new MockFileData(
                @$"{{
                      ""name"": ""cmf.docs.area"",
                      ""version"": ""{version}"",
                      ""description"": ""Help customization package"",
                      ""private"": true,
                      ""scripts"": {{
                        ""preinstall"": ""node npm.preinstall.js"",
                        ""postinstall"": ""node npm.postinstall.js""
                      }},
                      ""repository"": {{
                        ""type"": ""git"",
                        ""url"": ""https://url/git""
                      }}
                }}")
            },
            { metadataTS, new MockFileData(
                @$"
                (...)
                function applyConfig (packageName: string) {{
                  const config: PackageMetadata = {{
                    version: {quoteType}{version}{quoteType},
                (...)
            ")
            }
        });

        ExecutionContext.ServiceProvider = (new ServiceCollection())
            .AddSingleton<IProjectConfigService>(new ProjectConfigService())
            .BuildServiceProvider();
        ExecutionContext.Initialize(fileSystem);

        IFileInfo cmfpackageFile = fileSystem.FileInfo.New(cmfPackageJson);
        IPackageTypeHandler packageTypeHandler = PackageTypeFactory.GetPackageTypeHandler(cmfpackageFile);
        packageTypeHandler.Bump(bumpVersion, "");

        string cmfPackageVersion = (packageTypeHandler as HelpGulpPackageTypeHandler).CmfPackage.Version;
        dynamic packageFile = JsonConvert.DeserializeObject(fileSystem.File.ReadAllText(npmPackageJson));
        string packageFileVersion = packageFile.version;
        string metadataFile = fileSystem.File.ReadAllText(metadataTS);

        cmfPackageVersion.Should().Be(bumpVersion);
        packageFileVersion.Should().Be(bumpVersion);
        metadataFile.Should().Contain($"version: \"{bumpVersion}\"");
    }
}
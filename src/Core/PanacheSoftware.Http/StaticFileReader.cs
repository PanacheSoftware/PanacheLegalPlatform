using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Http
{
    public class StaticFileReader : IStaticFileReader
    {
        private IWebHostEnvironment _env;

        public StaticFileReader(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string GetJSONSeedData(SeedType seedType, SeedVersion seedVersion)
        {
            var seedFileURI = GenerateSeedURI(seedType, seedVersion);

            if(!string.IsNullOrWhiteSpace(seedFileURI))
            {
                var fileContent = System.IO.File.ReadAllText(seedFileURI);

                if(!string.IsNullOrWhiteSpace(fileContent))
                {
                    return fileContent;
                }
            }

            return string.Empty;
        }

        private string GenerateSeedURI(SeedType seedType, SeedVersion seedVersion)
        {
            var seedDirectory = DirectoryForSeedType(seedType);

            if(!string.IsNullOrWhiteSpace(seedDirectory))
            {
                var seedFile = FileNameForSeed(seedType, seedVersion);

                if(!string.IsNullOrWhiteSpace(seedFile))
                {
                    return $"{seedDirectory}{seedFile}";
                }
            }

            return string.Empty;
        }

        private string DirectoryForSeedType(SeedType seedType)
        {
            var seedDirectory = string.Empty;

            switch (seedType)
            {
                case SeedType.LANGUAGE:
                    seedDirectory = "Language";
                    break;
                case SeedType.SETTING:
                    seedDirectory = "Setting";
                    break;
                default:
                    break;
            }

            if(!string.IsNullOrWhiteSpace(seedDirectory))
            {
                return $"{_env.ContentRootPath}/Data/Seeds/{seedDirectory}/";
            }

            return string.Empty;
        }

        private string FileNameForSeed(SeedType seedType, SeedVersion seedVersion)
        {
            StringBuilder filename = new StringBuilder();

            var filePrefix = string.Empty;
            var fileVersion = string.Empty;

            switch (seedType)
            {
                case SeedType.LANGUAGE:
                    filePrefix = "language-seed";
                    break;
                case SeedType.SETTING:
                    filePrefix = "setting-seed";
                    break;
                default:
                    break;
            }

            switch (seedVersion)
            {
                case SeedVersion.V1:
                    fileVersion = "v1";
                    break;
                default:
                    break;
            }

            if(!string.IsNullOrWhiteSpace(filePrefix) && !string.IsNullOrWhiteSpace(fileVersion))
            {
                return $"{filePrefix}-{fileVersion}.json";
            }

            return string.Empty;
        }
    }
}

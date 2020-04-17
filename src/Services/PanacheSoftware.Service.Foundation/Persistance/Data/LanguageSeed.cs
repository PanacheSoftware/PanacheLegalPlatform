using PanacheSoftware.Core.Domain.Language;
using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using PanacheSoftware.Core.Domain.API.Language;
using Microsoft.AspNetCore.Http;
using PanacheSoftware.Http;

namespace PanacheSoftware.Service.Foundation.Persistance.Data
{
    public class LanguageSeed
    {
        private readonly IStaticFileReader _staticFileReader;

        public LanguageSeed(IStaticFileReader staticFileReader)
        {
            _staticFileReader = staticFileReader;
        }

        public LangSeed SeedData()
        {
            var seedData = _staticFileReader.GetJSONSeedData(SeedType.LANGUAGE, SeedVersion.V1);

            var langSeed = new LangSeed();

            if (!string.IsNullOrWhiteSpace(seedData))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                langSeed = JsonSerializer.Deserialize<LangSeed>(seedData, options);

                var test = string.Empty;
            }

            return langSeed;
        }
    }
}

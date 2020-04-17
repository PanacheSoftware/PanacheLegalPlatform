using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Persistance.Data
{
    public class SettingsSeed
    {
        private readonly IStaticFileReader _staticFileReader;

        public SettingsSeed(IStaticFileReader staticFileReader)
        {
            _staticFileReader = staticFileReader;
        }

        public SettingSeed SeedData()
        {
            var seedData = _staticFileReader.GetJSONSeedData(SeedType.SETTING, SeedVersion.V1);

            //var settingSeedtmp = new SettingSeed();

            //settingSeedtmp.SettingHeaders.Add(new SettingHead()
            //{
            //    Name = "USER_LANGUAGE",
            //    Value = "EN",
            //    DefaultValue = "EN",
            //    Description = "User Language",
            //    SettingType = "USER",
            //    Status = "N"
            //});

            //settingSeedtmp.SettingHeaders.Add(new SettingHead()
            //{
            //    Name = "SYSTEM_LANGUAGE",
            //    Value = "EN",
            //    DefaultValue = "EN",
            //    Description = "System Language",
            //    SettingType = "SYSTEM",
            //    Status = "N"
            //});

            //var options = new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //    WriteIndented = true
            //};

            //var seedString = JsonSerializer.Serialize<SettingSeed>(settingSeedtmp, options);


            var settingSeed = new SettingSeed();

            if (!string.IsNullOrWhiteSpace(seedData))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                settingSeed = JsonSerializer.Deserialize<SettingSeed>(seedData, options);

                var test = string.Empty;
            }

            return settingSeed;
        }
    }
}

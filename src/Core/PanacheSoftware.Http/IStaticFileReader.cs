using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Http
{
    public interface IStaticFileReader
    {
        string GetJSONSeedData(SeedType seedType, SeedVersion seedVersion);
    }
}

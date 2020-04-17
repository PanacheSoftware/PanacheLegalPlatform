using PanacheSoftware.Core.Domain.Language;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Foundation.Core.Repositories;
using PanacheSoftware.Service.Foundation.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Persistance.Repositories
{
    public class LanguageItemRepository : PanacheSoftwareRepository<LanguageItem>, ILanguageItemRepository
    {
        public LanguageItemRepository(PanacheSoftwareServiceFoundationContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceFoundationContext PanacheLegalFoundationContext
        {
            get { return Context as PanacheSoftwareServiceFoundationContext; }
        }
    }
}

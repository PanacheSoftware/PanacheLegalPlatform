using Microsoft.EntityFrameworkCore;
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
    public class LanguageCodeRepository : PanacheSoftwareRepository<LanguageCode>, ILanguageCodeRepository
    {
        public LanguageCodeRepository(PanacheSoftwareServiceFoundationContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceFoundationContext PanacheLegalFoundationContext
        {
            get { return Context as PanacheSoftwareServiceFoundationContext; }
        }

        public LanguageCode GetLanguageCode(string languageCodeId, bool readOnly = true)
        {
            if(Guid.TryParse(languageCodeId, out Guid foundId))
            {
                return GetLanguageCode(foundId, readOnly);
            }

            if (readOnly)
                return PanacheLegalFoundationContext.LanguageCodes
                .AsNoTracking()
                .SingleOrDefault(c => c.LanguageCodeId == languageCodeId);

            return PanacheLegalFoundationContext.LanguageCodes
                .SingleOrDefault(c => c.LanguageCodeId == languageCodeId);
        }

        public LanguageCode GetLanguageCode(Guid Id, bool readOnly = true)
        {
            if (readOnly)
                return PanacheLegalFoundationContext.LanguageCodes
                .AsNoTracking()
                .SingleOrDefault(c => c.Id == Id);

            return PanacheLegalFoundationContext.LanguageCodes
                .SingleOrDefault(c => c.Id == Id);
        }
    }
}

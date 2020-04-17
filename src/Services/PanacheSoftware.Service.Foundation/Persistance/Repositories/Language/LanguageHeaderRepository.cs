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
    public class LanguageHeaderRepository : PanacheSoftwareRepository<LanguageHeader>, ILanguageHeaderRepository
    {
        public LanguageHeaderRepository(PanacheSoftwareServiceFoundationContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceFoundationContext PanacheLegalFoundationContext
        {
            get { return Context as PanacheSoftwareServiceFoundationContext; }
        }

        public LanguageHeader GetLanguageHeaderWithRelations(Guid languageHeaderId, bool readOnly = true)
        {
            if (readOnly)
                return PanacheLegalFoundationContext.LanguageHeaders
                .Include(l => l.LanguageItems)
                .AsNoTracking()
                .SingleOrDefault(c => c.Id == languageHeaderId);

            return PanacheLegalFoundationContext.LanguageHeaders
                .Include(l => l.LanguageItems)
                .SingleOrDefault(c => c.Id == languageHeaderId);
        }

        public LanguageHeader GetLanguageHeaderWithRelations(long languageTextCode, bool readOnly = true)
        {
            if (readOnly)
                return PanacheLegalFoundationContext.LanguageHeaders
                .Include(l => l.LanguageItems)
                .AsNoTracking()
                .SingleOrDefault(c => c.TextCode == languageTextCode);

            return PanacheLegalFoundationContext.LanguageHeaders
                .Include(l => l.LanguageItems)
                .SingleOrDefault(c => c.TextCode == languageTextCode);
        }
    }
}

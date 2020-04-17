using PanacheSoftware.Core.Domain.Language;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Core.Repositories
{
    public interface ILanguageHeaderRepository : IPanacheSoftwareRepository<LanguageHeader>
    {
        LanguageHeader GetLanguageHeaderWithRelations(Guid languageHeaderId, bool readOnly = true);
        LanguageHeader GetLanguageHeaderWithRelations(long languageTextCode, bool readOnly = true);
    }
}

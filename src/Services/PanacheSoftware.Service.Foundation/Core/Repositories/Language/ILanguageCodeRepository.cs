using PanacheSoftware.Core.Domain.Language;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Core.Repositories
{
    public interface ILanguageCodeRepository : IPanacheSoftwareRepository<LanguageCode>
    {
        LanguageCode GetLanguageCode(Guid Id, bool readOnly = true);
        LanguageCode GetLanguageCode(string languageCodeId, bool readOnly = true);
    }
}

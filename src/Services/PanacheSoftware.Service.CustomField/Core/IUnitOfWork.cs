using PanacheSoftware.Database.Core;
using PanacheSoftware.Service.CustomField.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Core
{
    public interface IUnitOfWork : IPanacheSoftwareUnitOfWork
    {
        ICustomFieldHeaderRepository CustomFieldHeaders { get; }
        ICustomFieldGroupDetailRepository CustomFieldGroupDetails { get; }
        ICustomFieldGroupHeaderRepository CustomFieldGroupHeaders { get; }
        ICustomFieldTagRepository CustomFieldTags { get; }
        ICustomFieldValueRepository CustomFieldValues { get; }
        ICustomFieldValueHistoryRepository CustomFieldValueHistorys { get; }
    }
}

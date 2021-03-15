using PanacheSoftware.Database.Persistance;
using PanacheSoftware.Http;
using PanacheSoftware.Service.CustomField.Core;
using PanacheSoftware.Service.CustomField.Core.Repositories;
using PanacheSoftware.Service.CustomField.Persistance.Context;
using PanacheSoftware.Service.CustomField.Persistance.Repositories.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.Repositories
{
    public class UnitOfWork : PanacheSoftwareUnitOfWork, IUnitOfWork
    {
        public UnitOfWork(PanacheSoftwareServiceCustomFieldContext context, IUserProvider userProvider) : base(context, userProvider)
        {
            CustomFieldHeaders = new CustomFieldHeaderRepository(context);
            CustomFieldGroupDetails = new CustomFieldGroupDetailRepository(context);
            CustomFieldGroupHeaders = new CustomFieldGroupHeaderRepository(context);
            CustomFieldTags = new CustomFieldTagRepository(context);
            CustomFieldValues = new CustomFieldValueRepository(context);
            CustomFieldValueHistorys = new CustomFieldValueHistoryRepository(context);
            CustomFieldGroupLinks = new CustomFieldGroupLinkRepository(context);
        }

        public ICustomFieldHeaderRepository CustomFieldHeaders { get; private set; }
        public ICustomFieldGroupDetailRepository CustomFieldGroupDetails { get; private set; }
        public ICustomFieldGroupHeaderRepository CustomFieldGroupHeaders { get; private set; }
        public ICustomFieldTagRepository CustomFieldTags { get; private set; }
        public ICustomFieldValueRepository CustomFieldValues { get; private set; }
        public ICustomFieldValueHistoryRepository CustomFieldValueHistorys { get; private set; }
        public ICustomFieldGroupLinkRepository CustomFieldGroupLinks { get; private set; }
    }
}

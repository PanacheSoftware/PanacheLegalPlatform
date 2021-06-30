using PanacheSoftware.Core.Domain.API.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Manager
{
    public interface ICustomFieldManager
    {
        Tuple<CustomFieldVal, string> CreateCustomFieldValue(CustomFieldVal customFieldVal);
        void CheckForAndCreateHistory(Guid customFieldValueId, string historicValue, DateTime historicLastUpdate);
    }
}

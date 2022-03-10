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
        bool CustomFieldGroupShortNameExists(string shortName);
        void SetCustomFieldShortNames(CustomFieldGroupHead customFieldGroupHead);
        //string CreateFieldShortName(string longName, int counter = 0);
        IList<string> DuplicateShortNames(CustomFieldGroupHead customFieldGroupHead);
        bool DuplicateFieldShortName(IList<CustomFieldHead> customFieldHeads, string shortname);
        string SetCustomFieldShortName(CustomFieldHead customFieldHead, CustomFieldGroupHead customFieldGroupHead);
        bool BlankShortNames(IList<CustomFieldHead> customFieldHeads);
    }
}

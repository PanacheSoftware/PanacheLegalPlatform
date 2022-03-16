using AutoMapper;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.CustomField;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Core.Helper;
using PanacheSoftware.Service.CustomField.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Manager
{
    public class CustomFieldManager : ICustomFieldManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomFieldManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Tuple<CustomFieldVal, string> CreateCustomFieldValue(CustomFieldVal customFieldVal)
        {
            if (customFieldVal.Id != Guid.Empty)
                return new Tuple<CustomFieldVal, string>(default, $"Id: Does not equal empty guid.");

            var foundCustomFieldHeader = _unitOfWork.CustomFieldHeaders.GetCustomFieldHeader(customFieldVal.CustomFieldHeaderId, true);

            if(foundCustomFieldHeader == default)
                return new Tuple<CustomFieldVal, string>(default, $"CustomFieldHeaderId: Custom Field Header not found.");

            if(!_unitOfWork.CustomFieldGroupLinks.CustomFieldGroupLinkExists(customFieldVal.LinkId, customFieldVal.LinkType, foundCustomFieldHeader.CustomFieldGroupHeaderId))
                return new Tuple<CustomFieldVal, string>(default, $"LinkId: Custom Field is not associated with LinkId.");

            if(_unitOfWork.CustomFieldValues.GetCustomFieldValue(customFieldVal.LinkId, customFieldVal.LinkType, foundCustomFieldHeader.CustomFieldGroupHeaderId, true) != default)
                return new Tuple<CustomFieldVal, string>(default, $"LinkId: Custom Field value already exists for this LinkId.");

            var customFieldValue = _mapper.Map<CustomFieldValue>(customFieldVal);

            _unitOfWork.CustomFieldValues.Add(customFieldValue);

            _unitOfWork.Complete();

            return new Tuple<CustomFieldVal, string>(_mapper.Map<CustomFieldVal>(customFieldValue), string.Empty);
        }

        public void CheckForAndCreateHistory(Guid customFieldValueId, string historicValue, DateTime historicLastUpdate)
        {
            if (customFieldValueId == Guid.Empty)
                return;

            var customFieldValue = _unitOfWork.CustomFieldValues.Get(customFieldValueId);

            if (customFieldValue == default)
                return;

            if (customFieldValue.FieldValue == historicValue)
                return;

            var foundCustomFieldHeader = _unitOfWork.CustomFieldHeaders.GetCustomFieldHeader(customFieldValue.CustomFieldHeaderId, true);

            if (foundCustomFieldHeader == default)
                return;

            if (!foundCustomFieldHeader.History)
                return;

            var fieldHistorys = _unitOfWork.CustomFieldValueHistorys.GetCustomFieldValueHistorys(customFieldValue.Id, true);
            var nextSequenceNo = 0;

            if(fieldHistorys.Any())
            {
                var maxHistory = fieldHistorys.OrderByDescending(h => h.SequenceNo).FirstOrDefault();

                if (maxHistory != default)
                    nextSequenceNo = maxHistory.SequenceNo + 1;
            }

            var newCustomFieldHistory = new CustomFieldValueHistory()
            {
                CustomFieldValueId = customFieldValue.Id,
                OriginalCreationDate = historicLastUpdate,
                FieldValue = historicValue,
                SequenceNo = nextSequenceNo,
                Status = StatusTypes.Open
            };

            _unitOfWork.CustomFieldValueHistorys.Add(newCustomFieldHistory);

            _unitOfWork.Complete();
        }

        public bool CustomFieldGroupShortNameExists(string shortName)
        {
            var foundShortName = _unitOfWork.CustomFieldGroupHeaders.SingleOrDefault(c => c.ShortName == shortName, true);

            return foundShortName != default;
        }

        public void SetCustomFieldShortNames(CustomFieldGroupHead customFieldGroupHead)
        {
            foreach (var customFieldHead in customFieldGroupHead.CustomFieldHeaders)
            {
                if (string.IsNullOrWhiteSpace(customFieldHead.ShortName))
                {
                    customFieldHead.ShortName = SetCustomFieldShortName(customFieldHead, customFieldGroupHead);
                }
            }
        }

        public string SetCustomFieldShortName(CustomFieldHead customFieldHead, CustomFieldGroupHead customFieldGroupHead)
        {
            var newShortName = customFieldHead.ShortName;

            if (string.IsNullOrWhiteSpace(customFieldHead.ShortName))
            {
                for (int i = 0; i <= 999; i++)
                {
                    newShortName = Naming.CreateFieldShortName(customFieldHead.Name, i);

                    if (!DuplicateFieldShortName(customFieldGroupHead.CustomFieldHeaders, newShortName))
                        break;

                    if (i == 999)
                        newShortName = string.Empty;
                }
            }

            return newShortName;
        }

        //public string CreateFieldShortName(string longName, int counter = 0)
        //{
        //    if (string.IsNullOrWhiteSpace(longName))
        //        return string.Empty;

        //    var shortName = longName.ToUpper();
        //    shortName = Regex.Replace(shortName, "[^A-Z0-9_.]+", "", RegexOptions.Compiled);

        //    var counterLength = counter <= 1 ? 0 : counter.ToString().Length;
        //    var maxLength = 100 - counterLength;

        //    if(shortName.Length > maxLength)
        //        shortName = shortName.Substring(0, maxLength);

        //    if (counter > 0)
        //        shortName = $"{shortName}{counter}";

        //    return shortName;
        //}

        public bool DuplicateFieldShortName(IList<CustomFieldHead> customFieldHeads, string shortname)
        {
            var foundShortName = customFieldHeads.FirstOrDefault(c => c.ShortName == shortname);

            return foundShortName != default;
        }

        public bool BlankShortNames(IList<CustomFieldHead> customFieldHeads)
        {
            var blankShortNames = customFieldHeads.FirstOrDefault(c => c.ShortName == string.Empty);

            return blankShortNames != default;
        }

        public IList<string> DuplicateShortNames(CustomFieldGroupHead customFieldGroupHead)
        {
            var duplicateShortNames = new List<string>();
            var groupedByShortName = customFieldGroupHead.CustomFieldHeaders.GroupBy(x => x.ShortName);
            var duplicates = groupedByShortName.Where(item => item.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                if(duplicate.Count() > 0)
                    duplicateShortNames.Add(duplicate.Key);
            }

            return duplicateShortNames;
        }
    }
}

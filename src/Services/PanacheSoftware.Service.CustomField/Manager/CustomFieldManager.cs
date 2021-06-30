using AutoMapper;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.CustomField;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Service.CustomField.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async void CheckForAndCreateHistory(Guid customFieldValueId, string historicValue, DateTime historicLastUpdate)
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
    }
}

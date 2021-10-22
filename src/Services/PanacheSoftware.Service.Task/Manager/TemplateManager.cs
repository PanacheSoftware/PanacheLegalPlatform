using AutoMapper;
using PanacheSoftware.Core.Domain.API.Task.Template;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Task.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Manager
{
    public class TemplateManager : ITemplateManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;
        private readonly IAPIHelper _apiHelper;
        private List<Guid> _userTeams;

        public TemplateManager(IUnitOfWork unitOfWork, IMapper mapper, IUserProvider userProvider, IAPIHelper apiHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
            _apiHelper = apiHelper;
        }

        public async Task<TemplateHeadList> GetTemplateHeadListAsync(string accessToken)
        {
            //var userTeams = await GetUserTeamsAsync(accessToken);

            var templateHeadList = new TemplateHeadList();

            foreach (var currentTemplateHeader in _unitOfWork.TemplateHeaders.GetAll(true))
            {
                templateHeadList.TemplateHeaders.Add(_mapper.Map<TemplateHead>(currentTemplateHeader));
            }

            return templateHeadList;
        }

        private async Task<List<Guid>> GetUserTeamsAsync(string accessToken)
        {
            if (_userTeams == null)
                _userTeams = await _apiHelper.GetTeamsForUserId(accessToken, _userProvider.GetUserId());

            return _userTeams;
        }
    }
}

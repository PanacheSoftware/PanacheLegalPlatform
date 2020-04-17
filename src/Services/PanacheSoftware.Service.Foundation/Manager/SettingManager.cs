using AutoMapper;
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Foundation.Core;
using PanacheSoftware.Service.Foundation.Persistance.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Manager
{
    public class SettingManager : ISettingManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStaticFileReader _staticFileReader;

        public SettingManager(IUnitOfWork unitOfWork, IMapper mapper, IStaticFileReader staticFileReader)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _staticFileReader = staticFileReader;
        }

        public void SeedSettings()
        {
            if (!_unitOfWork.SettingHeaders.Any())
            {
                var settingsSeed = new SettingsSeed(_staticFileReader);

                var settingSeed = settingsSeed.SeedData();

                if (!_unitOfWork.SettingHeaders.Any())
                {
                    if (settingSeed.SettingHeaders.Any())
                    {
                        foreach (var settingHeader in settingSeed.SettingHeaders)
                        {
                            _unitOfWork.SettingHeaders.Add(_mapper.Map<SettingHeader>(settingHeader));
                        }

                        _unitOfWork.Complete();
                    }
                }
            }
        }
    }
}

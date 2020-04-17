using AutoMapper;
using Microsoft.AspNetCore.Http;
using PanacheSoftware.Core.Domain.Language;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Foundation.Core;
using PanacheSoftware.Service.Foundation.Persistance.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Manager
{
    public class LanguageManager : ILanguageManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStaticFileReader _staticFileReader;


        public LanguageManager(IUnitOfWork unitOfWork, IMapper mapper, IStaticFileReader staticFileReader)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _staticFileReader = staticFileReader;
        }

        public void SeedLanguage()
        {
            if (!_unitOfWork.LanguageCodes.Any() || !_unitOfWork.LanguageHeaders.Any())
            {
                var languageSeed = new LanguageSeed(_staticFileReader);

                var langSeed = languageSeed.SeedData();

                if(!_unitOfWork.LanguageCodes.Any())
                {
                    if(langSeed.LangCodes.Any())
                    {
                        foreach (var languageCode in langSeed.LangCodes)
                        {
                            _unitOfWork.LanguageCodes.Add(_mapper.Map<LanguageCode>(languageCode));
                        }

                        _unitOfWork.Complete();
                    }
                }

                if(!_unitOfWork.LanguageHeaders.Any())
                {
                    if(langSeed.LangHeaders.Any())
                    {
                        foreach (var languageHeader in langSeed.LangHeaders)
                        {
                            _unitOfWork.LanguageHeaders.Add(_mapper.Map<LanguageHeader>(languageHeader));
                        }

                        _unitOfWork.Complete();
                    }
                }
            }
        }
    }
}

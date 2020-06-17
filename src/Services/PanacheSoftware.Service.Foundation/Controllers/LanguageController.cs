using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.Language;
using PanacheSoftware.Service.Foundation.Core;

namespace PanacheSoftware.Service.Foundation.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("[controller]")]
    public class LanguageController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LanguageController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult Get()
        {
            try
            {
                LangList langList = new LangList();

                foreach (var currentLang in _unitOfWork.LanguageHeaders.GetAll(true))
                {
                    langList.LanguageHeaders.Add(_mapper.Map<LangHead>(currentLang));
                }

                if (langList.LanguageHeaders.Count > 0)
                    return Ok(langList);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult Get(string id)
        {
            try
            {
                LanguageHeader languageHeader;

                if (Guid.TryParse(id, out Guid foundId))
                {
                    languageHeader = _unitOfWork.LanguageHeaders.GetLanguageHeaderWithRelations(foundId);
                }
                else
                {
                    if (long.TryParse(id, out long foundTextCode))
                    {
                        languageHeader = _unitOfWork.LanguageHeaders.GetLanguageHeaderWithRelations(foundTextCode);
                    }
                    else
                    {
                        languageHeader = null;
                    }
                }

                if (languageHeader != null)
                {
                    return Ok(_mapper.Map<LangHead>(languageHeader));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]LangHead langHead)
        {
            try
            {
                if (langHead.Id == Guid.Empty)
                {
                    if (langHead.TextCode < 10000 || langHead.TextCode > 19999)
                    {
                        var foundExisting = _unitOfWork.LanguageHeaders.GetLanguageHeaderWithRelations(langHead.TextCode);

                        if (foundExisting == null)
                        {
                            //var userId = User.FindFirstValue("sub");

                            var languageHeader = _mapper.Map<LanguageHeader>(langHead);

                            _unitOfWork.LanguageHeaders.Add(languageHeader);

                            _unitOfWork.Complete();

                            return Created(new Uri($"{Request.Path}/{languageHeader.Id}", UriKind.Relative), _mapper.Map<LangHead>(languageHeader));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult Patch(string id, [FromBody]JsonPatchDocument<LangHead> langHeadPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    LanguageHeader languageHeader = _unitOfWork.LanguageHeaders.Get(parsedId);

                    LangHead langHead = _mapper.Map<LangHead>(languageHeader);

                    langHeadPatch.ApplyTo(langHead);

                    _mapper.Map(langHead, languageHeader);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Get", new { id = _mapper.Map<LangHead>(languageHeader).Id }, _mapper.Map<LangHead>(languageHeader));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult Item(string id)
        {
            try
            {
                LanguageItem languageItem;

                if (Guid.TryParse(id, out Guid foundId))
                {
                    languageItem = _unitOfWork.LanguageItems.Get(foundId);
                }
                else
                {
                    languageItem = null;
                }

                if (languageItem != null)
                {
                    return Ok(_mapper.Map<LangItem>(languageItem));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Item([FromBody]LangItem langItem)
        {
            try
            {
                if (langItem.Id == Guid.Empty)
                {
                    //var userId = User.FindFirstValue("sub");

                    var languageItem = _mapper.Map<LanguageItem>(langItem);

                    _unitOfWork.LanguageItems.Add(languageItem);

                    _unitOfWork.Complete();

                    return Created(new Uri($"{Request.Path}/Item/{languageItem.Id}", UriKind.Relative), _mapper.Map<LangItem>(languageItem));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [Route("[action]/{id}")]
        [HttpPatch]
        public IActionResult Item(string id, [FromBody]JsonPatchDocument<LangItem> langItemPatch)
        {
            try
            {
                if (Guid.TryParse(id, out Guid parsedId))
                {
                    //var userId = User.FindFirstValue("sub");

                    LanguageItem languageItem = _unitOfWork.LanguageItems.Get(parsedId);

                    LangItem langItem = _mapper.Map<LangItem>(languageItem);

                    langItemPatch.ApplyTo(langItem);

                    _mapper.Map(langItem, languageItem);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Item", new { id = _mapper.Map<LangItem>(languageItem).Id }, _mapper.Map<LangItem>(languageItem));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public IActionResult Code(string id)
        {
            try
            {
                LanguageCode languageCode = _unitOfWork.LanguageCodes.GetLanguageCode(id);

                if (languageCode != null)
                {
                    return Ok(_mapper.Map<LangCode>(languageCode));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult Code()
        {
            try
            {
                LangCodeList langCodeList = new LangCodeList();

                foreach (var currentLangCode in _unitOfWork.LanguageCodes.GetAll(true))
                {
                    langCodeList.LanguageCodes.Add(_mapper.Map<LangCode>(currentLangCode));
                }

                if (langCodeList.LanguageCodes.Count > 0)
                    return Ok(langCodeList);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Code([FromBody]LangCode langCode)
        {
            try
            {
                if (langCode.Id == Guid.Empty)
                {
                    var foundLangCode = _unitOfWork.LanguageCodes.GetLanguageCode(langCode.LanguageCodeId);

                    if(foundLangCode == null)
                    {
                        var languageCode = _mapper.Map<LanguageCode>(langCode);

                        _unitOfWork.LanguageCodes.Add(languageCode);

                        _unitOfWork.Complete();

                        return Created(new Uri($"{Request.Path}/Code/{languageCode.Id}", UriKind.Relative), _mapper.Map<LangItem>(languageCode));
                    }
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [Route("[action]/{id}")]
        [HttpPatch]
        public IActionResult Code(string id, [FromBody]JsonPatchDocument<LangCode> langCodePatch)
        {
            try
            {
                LanguageCode languageCode = _unitOfWork.LanguageCodes.GetLanguageCode(id, false);

                if (languageCode != null)
                {
                    LangCode langCode = _mapper.Map<LangCode>(languageCode);

                    langCodePatch.ApplyTo(langCode);

                    _mapper.Map(langCode, languageCode);

                    _unitOfWork.Complete();

                    return CreatedAtRoute("Code", new { id = _mapper.Map<LangCode>(languageCode).Id }, _mapper.Map<LangCode>(languageCode));
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return BadRequest();
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult LanguageQuery([FromBody]LangQueryList langQuerylist)
        {
            try
            {
                foreach (var langQuery in langQuerylist.LangQuerys)
                {
                    LanguageHeader foundHeader = _unitOfWork.LanguageHeaders.GetLanguageHeaderWithRelations(langQuery.TextCode);

                    if(foundHeader != null)
                    {
                        langQuery.Text = foundHeader.Text;

                        var languageItem = foundHeader.LanguageItems.Where(l => l.LanguageCodeId == langQuery.LanguageCode).FirstOrDefault();

                        if(languageItem != null)
                        {
                            langQuery.Text = languageItem.Text;
                        }
                    }
                }

                if (langQuerylist.LangQuerys.Count > 0)
                    return Ok(langQuerylist);
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return NotFound();
        }
    }
}
﻿using PanacheSoftware.Core.Domain.API.Language;
using System;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Core.Helpers
{
    public interface IModelHelper
    {
        Task<Tuple<bool, string>> ProcessPatchWithMessage(object existingObject, object updatedObject, Guid objectId, string accessToken, string apiType, string urlPrefix);
        Task<bool> ProcessPatch(object existingObject, object updatedObject, Guid objectId, string accessToken, string apiType, string urlPrefix);
        LangQueryList GetLangQueryList(string languageCode, long[] TextCodes);
    }
}

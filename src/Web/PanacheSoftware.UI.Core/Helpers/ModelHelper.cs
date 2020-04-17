﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Http;

namespace PanacheSoftware.UI.Core.Helpers
{
    public class ModelHelper : IModelHelper
    {
        private readonly IAPIHelper _apiHelper;

        public ModelHelper(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public LangQueryList GetLangQueryList(string languageCode, long[] TextCodes)
        {
            var langQueryList = new LangQueryList();

            foreach (var textCode in TextCodes)
            {
                var langQuery = new LangQuery()
                {
                    LanguageCode = languageCode,
                    TextCode = textCode
                };
                langQueryList.LangQuerys.Add(langQuery);
            }

            return langQueryList;
        }

        public async Task<bool> ProcessPatch(object existingObject, object updatedObject, Guid objectId, string accessToken, string apiType, string urlPrefix)
        {
            var jsonPatchDocument = new JsonPatchDocument();

            if (jsonPatchDocument.GeneratePatch(existingObject, updatedObject))
            {
                var test = JsonConvert.SerializeObject(jsonPatchDocument);
                HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");

                try
                {
                    var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Patch, apiType, $"{urlPrefix}/{objectId.ToString()}", contentPost);
                    return true;
                }
                catch (Exception ex)
                {
                    var responseString = $"Error calling API: {ex.Message}";
                    return false;
                }
            }

            return true;
        }
    }
}

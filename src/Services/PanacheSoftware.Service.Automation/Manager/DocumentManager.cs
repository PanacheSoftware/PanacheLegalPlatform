using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using PanacheSoftware.Core.Domain.API.Automation;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Automation.Manager
{
    public class DocumentManager : IDocumentManager
    {
        private readonly IAPIHelper _apiHelper;

        public DocumentManager(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<Tuple<bool, string>> AutomateDocument(AutoDoc autoDoc, string accessToken)
        {
            byte[] documentToProcess = autoDoc.Content;

            if(autoDoc.Content == null)
            {
                await GetFile(autoDoc, accessToken);

                if(autoDoc.Content == null)
                    return new Tuple<bool, string>(false, "Did not find file to process");
            }

            autoDoc.Content = ProcessDocument(autoDoc.Content, autoDoc.AutoValues, autoDoc.OpenVariableSymbol, autoDoc.CloseVariableSymbol);

            return new Tuple<bool, string>(true, String.Empty);
        }

        public byte[] ProcessDocument(byte[] documentToProcess, IList<AutoVal> values, string openVariableSymbol, string closeVariableSymbol)
        {
            var openXMLDoc = new OpenXmlPowerToolsDocument(documentToProcess);

            var wmlDoc = new WmlDocument(openXMLDoc);

            foreach (var autoVal in values)
            {
                wmlDoc = wmlDoc.SearchAndReplace($"{openVariableSymbol}{autoVal.Placeholder}{closeVariableSymbol}", autoVal.Value, false);
            }

            return wmlDoc.DocumentByteArray;
        }

        private async Task GetFile(AutoDoc autoDoc, string accessToken)
        {
            var URIPart = string.Empty;

            if (autoDoc.FileVersionId != Guid.Empty)
                URIPart = $"File/Version/{autoDoc.FileVersionId}";

            if (string.IsNullOrWhiteSpace(URIPart) && autoDoc.FileHeaderId != Guid.Empty)
                URIPart = $"File/Version/GetFileLatestVersion/{autoDoc.FileHeaderId}";

            if (!string.IsNullOrWhiteSpace(URIPart))
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FILE, URIPart);

                if (response.IsSuccessStatusCode)
                {
                    var fileVersion = response.ContentAsType<FileVer>();
                    autoDoc.TrustedName = fileVersion.TrustedName;
                    autoDoc.FileType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    autoDoc.Content = fileVersion.Content;
                }
            }
        }
    }
}

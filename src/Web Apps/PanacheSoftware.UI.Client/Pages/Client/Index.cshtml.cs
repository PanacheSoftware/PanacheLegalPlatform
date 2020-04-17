using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Headers;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.Client
{
    [SecurityHeaders]
    [Authorize]
    [ValidateAntiForgeryToken]
    public class IndexModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        [BindProperty]
        public ClientHead clientHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public SelectList StatusList { get; }
        public SelectList TitleList { get; }
        public SelectList AddressTypeList { get; }
        public ClientHeaderModel clientHeaderModel { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }

        //public ClientHeaderModel dummyClientHeaderModel { get; set; }

        public IndexModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;

            //SelectList creation
            StatusTypes statusTypes = new StatusTypes();
            ContactTitles contactTitles = new ContactTitles();
            AddressTypes addressTypes = new AddressTypes();
            StatusList = new SelectList(statusTypes.GetStatusTypesDictionary(), "Key", "Value");
            TitleList = new SelectList(contactTitles.GetContactTitlesDictionary(), "Key", "Value");
            AddressTypeList = new SelectList(addressTypes.GetAddressTypesDictionary(), "Key", "Value");
        }
        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10300, 10500, 10501, 10310, 10301, 10302, 10303, 10202, 10203, 10221, 10205, 10200, 10201, 10304, 10305, 10309, 10308, 10206,
            10207, 10208, 10209, 10210, 10211, 10306, 10212, 10213, 10214, 10215, 10216, 10217, 10307});

            await GeneratePageConstructionModel();

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            clientHead = new ClientHead();

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            if (!string.IsNullOrWhiteSpace(Id))
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CLIENT, $"Client/{Id}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    clientHead = response.ContentAsType<ClientHead>();
                }
            }

            await PageConstructor(SaveStates.IGNORE, accessToken);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            if (ModelState.IsValid)
            {
                if (clientHead != null)
                {
                    if(await CreateOrUpdateClientAsync(accessToken))
                    {
                        await PageConstructor(SaveStates.SUCCESS, accessToken);
                    }
                    else
                    {
                        await PageConstructor(SaveStates.FAILED, accessToken);
                    }

                }

                return Page();
            }

            await PageConstructor(SaveStates.FAILED, accessToken);

            return Page();
        }

        private async Task<bool> GeneratePageConstructionModel()
        {
            ClientHeaderModel dummyClientHeaderModel = new ClientHeaderModel()
            {
                clientHead = GenerateDummyClientHeadModel(),
                StatusList = StatusList,
                TitleList = TitleList,
                AddressTypeList = AddressTypeList,
                langQueryList = this.langQueryList
            };

            var contactCardHTML = await _renderer.RenderPartialToStringAsync("_ClientContact", dummyClientHeaderModel);

            contactCardHTML = contactCardHTML.Replace("\n", "").Replace("\r", "").Replace("\t", "").ToString();

            var addressTableHTML = _renderer.GetStringBetween(contactCardHTML, "<!--AddrTableRowsStart-->", "<!--AddrTableRowsEnd-->");

            List<string> addressListRows = addressTableHTML.TrimStart().TrimEnd().Replace("<td>", "").Split("</td>").ToList();

            for (int i = addressListRows.Count - 1; i >= 0; i--)
            {
                var eol = ",";

                addressListRows[i] = addressListRows[i].TrimStart().TrimEnd();
                if (string.IsNullOrWhiteSpace(addressListRows[i]))
                {
                    addressListRows.RemoveAt(i);
                    continue;
                }

                if (i == addressListRows.Count - 1)
                {
                    eol = "";
                }

                addressListRows[i] = addressListRows[i].Replace("clientHead_ClientContacts_0__ClientAddresses_0__", "clientHead_ClientContacts_' + contactNumber + '__ClientAddresses_' + rowCount + '__");
                addressListRows[i] = addressListRows[i].Replace("clientHead.ClientContacts[0].ClientAddresses[0]", "clientHead.ClientContacts[' + contactNumber + '].ClientAddresses[' + rowCount + ']");
                addressListRows[i] = addressListRows[i].Replace("11111111-1111-1111-1111-111111111111", "' + contactId + '");

                addressListRows[i] = $"'{addressListRows[i]}'{eol}";
            }

            dummyClientHeaderModel.clientHead.ClientContacts[0].ClientAddresses = new List<ClientAddr>();

            contactCardHTML = await _renderer.RenderPartialToStringAsync("_ClientContact", dummyClientHeaderModel);

            contactCardHTML = contactCardHTML.Replace("\n", "").Replace("\r", "").Replace("\t", "").ToString();

            contactCardHTML = contactCardHTML.Replace("collapse0", "collapse' + contactCount + '");
            contactCardHTML = contactCardHTML.Replace("heading0", "heading' + contactCount + '");
            contactCardHTML = contactCardHTML.Replace("_0__", "_' + contactCount + '__");
            contactCardHTML = contactCardHTML.Replace("[0]", "[' + contactCount + ']");
            contactCardHTML = contactCardHTML.Replace(".ClientHeaderId\" value=\"00000000-0000-0000-0000-000000000000\"", ".ClientHeaderId\" value=\"' + clientHeaderId + '\"");
            contactCardHTML = contactCardHTML.Replace("addr_table_00000000-0000-0000-0000-000000000000", "addr_table_' + newContactId + '");
            contactCardHTML = contactCardHTML.Replace("<input id=\"AddrTable_Contact_Num_00000000-0000-0000-0000-000000000000\" value=\"0\" hidden />", "<input id=\"AddrTable_Contact_Num_' + newContactId + '\" value=\"' + contactCount + '\" hidden />");
            contactCardHTML = contactCardHTML.Replace("AddrTableCount_00000000-0000-0000-0000-000000000000", "AddrTableCount_' + newContactId + '");
            contactCardHTML = contactCardHTML.Replace("add_address_00000000-0000-0000-0000-000000000000", "add_address_' + newContactId + '");

            clientHeaderModel = new ClientHeaderModel()
            {
                clientHead = clientHead,
                StatusList = StatusList,
                contactCardHTML = contactCardHTML,
                addressRows = addressListRows.ToArray(),
                TitleList = TitleList,
                AddressTypeList = AddressTypeList,
                langQueryList = this.langQueryList
            };

            return true;
        }

        //private static string GetStringBetween(string strSource, string strStartTag, string strEndTag)
        //{
        //    int start, end;
        //    if(strSource.Contains(strStartTag) && strSource.Contains(strEndTag))
        //    {
        //        start = strSource.IndexOf(strStartTag, 0) + strStartTag.Length;
        //        end = strSource.IndexOf(strEndTag, start);
        //        return strSource.Substring(start, end - start);
        //    }

        //    return string.Empty;
        //}

        private ClientHead GenerateDummyClientHeadModel()
        {
            var dummyClientHead = new ClientHead();

            dummyClientHead.Id = Guid.Empty;

            var dummyClientCon = new ClientCon()
            {
                Id = Guid.Empty,
                FirstName = string.Empty,
                MiddleName = string.Empty,
                LastName = string.Empty,
                Email = string.Empty,
                Phone = string.Empty
            };

            var dummyClientAddr = new ClientAddr()
            {
                ClientContactId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Id = Guid.Empty,
                AddressLine1 = string.Empty,
                AddressLine2 = string.Empty,
                AddressLine3 = string.Empty,
                AddressLine4 = string.Empty,
                AddressLine5 = string.Empty,
                Country = string.Empty,
                Email1 = string.Empty,
                Email2 = string.Empty,
                Email3 = string.Empty,
                Phone1 = string.Empty,
                Phone2 = string.Empty,
                Phone3 = string.Empty,
                PostalCode = string.Empty,
                Region = string.Empty

            };

            dummyClientCon.ClientAddresses.Add(dummyClientAddr);

            dummyClientHead.ClientContacts.Add(dummyClientCon);

            return dummyClientHead;
        }

        private async Task<bool> CreateOrUpdateClientAsync(string apiAccessToken)
        {
            if (clientHead != null)
            {
                if (clientHead.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.CLIENT, $"Client/{clientHead.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundClientHead = response.ContentAsType<ClientHead>();

                        if (foundClientHead != null)
                        {
                            if(!await _modelHelper.ProcessPatch(foundClientHead, clientHead, foundClientHead.Id, apiAccessToken, APITypes.CLIENT, "Client"))
                            {
                                return false;
                            }

                            response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.CLIENT, $"Detail/{clientHead.ClientDetail.Id}");

                            if(response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var foundClientDet = response.ContentAsType<ClientDet>();

                                if(foundClientDet != null)
                                {
                                    if (!await _modelHelper.ProcessPatch(foundClientDet, clientHead.ClientDetail, foundClientDet.Id, apiAccessToken, APITypes.CLIENT, "Detail"))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }

                        foreach (var clientCon in clientHead.ClientContacts)
                        {
                            if(!await CreateOrUpdateContactAsync(clientCon, apiAccessToken))
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(clientHead), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.CLIENT, $"Client", contentPost);

                        if(response.StatusCode != System.Net.HttpStatusCode.Created)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        var responseString = $"Error calling API: {ex.Message}";
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<bool> CreateOrUpdateContactAsync(ClientCon clientCon, string apiAccessToken)
        {
            if (clientCon != null)
            { 
                if (clientCon.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.CLIENT, $"Contact/{clientCon.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundClientCon = response.ContentAsType<ClientCon>();

                        if (foundClientCon != null)
                        {
                            if(!await _modelHelper.ProcessPatch(foundClientCon, clientCon, foundClientCon.Id, apiAccessToken, APITypes.CLIENT, "Contact"))
                            {
                                return false;
                            }
                        }

                        foreach (var clientAddr in clientCon.ClientAddresses)
                        {
                            if(!await CreateOrUpdateAddressAsync(clientAddr, apiAccessToken))
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(clientCon), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.CLIENT, $"Contact", contentPost);

                        if (response.StatusCode != System.Net.HttpStatusCode.Created)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        var responseString = $"Error calling API: {ex.Message}";
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<bool> CreateOrUpdateAddressAsync(ClientAddr clientAddr, string apiAccessToken)
        {
            if (clientAddr != null)
            {
                if (clientAddr.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.CLIENT, $"Address/{clientAddr.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundClientAddr = response.ContentAsType<ClientAddr>();

                        if (foundClientAddr != null)
                        {
                            if(!await _modelHelper.ProcessPatch(foundClientAddr, clientAddr, foundClientAddr.Id, apiAccessToken, APITypes.CLIENT, "Address"))
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(clientAddr), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.CLIENT, $"Address", contentPost);

                        if (response.StatusCode != System.Net.HttpStatusCode.Created)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        var responseString = $"Error calling API: {ex.Message}";
                        return false;
                    }
                }
            }

            return true;
        }

        //private async Task<bool> processPatch(object existingObject, object updatedObject, Guid objectId, string accessToken, string apiType, string urlPrefix)
        //{
        //    var jsonPatchDocument = new JsonPatchDocument();

        //    if (jsonPatchDocument.GeneratePatch(existingObject, updatedObject))
        //    {
        //        HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");

        //        try
        //        {
        //            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Patch, apiType, $"{urlPrefix}/{objectId.ToString()}", contentPost);
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            var responseString = $"Error calling API: {ex.Message}";
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}
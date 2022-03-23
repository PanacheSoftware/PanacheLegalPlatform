using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using PanacheSoftware.Core.Domain.API.Automation;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        public async Task<Tuple<bool, string>> AutomateDocumentFromLink(AutoDoc autoDoc, string accessToken, Guid linkId, string linkType)
        {
            if (autoDoc.Content == null)
            {
                await GetFile(autoDoc, accessToken);

                if (autoDoc.Content == null)
                    return new Tuple<bool, string>(false, "Did not find file to process");
            }

            await GetStartAndEndTags(autoDoc, accessToken);

            FindVariablesInDocument(autoDoc);

            if(autoDoc.AutoValues.Any())
            {
                await GetValuesFromTaskForAutoDoc(autoDoc, linkId, linkType, accessToken);
            }

            autoDoc.Content = ProcessDocument(autoDoc.Content, autoDoc.AutoValues, autoDoc.OpenVariableSymbol, autoDoc.CloseVariableSymbol);

            return new Tuple<bool, string>(true, String.Empty);
        }

        private async Task GetValuesFromTaskForAutoDoc(AutoDoc autoDoc, Guid linkId, string linkType, string accessToken)
        {
            var taskGroupHead = await GetTaskForLink(linkId, linkType, accessToken);

            if (taskGroupHead == null)
                return;

            foreach (var autoDocValue in autoDoc.AutoValues)
            {
                autoDocValue.Value = await GetValueFromTaskForTag(autoDocValue.Placeholder, taskGroupHead, accessToken);
            }
        }

        private async Task<string> GetValueFromTaskForTag(string tagData, TaskGroupHead taskGroupHead, string accessToken)
        {
            var stringValue = string.Empty;

            var autoDocTag = JsonSerializer.Deserialize<AutoDocTag>(tagData);

            if (autoDocTag == null)
                return string.Empty;

            stringValue = await GetHeaderValue(autoDocTag, taskGroupHead, accessToken);

            if(!string.IsNullOrWhiteSpace(stringValue))
                return stringValue;

            stringValue = await GetCustomFieldGroupValue(autoDocTag, taskGroupHead, accessToken);

            return stringValue;
        }

        private async Task<string> GetCustomFieldGroupValue(AutoDocTag autoDocTag, TaskGroupHead parentTaskGroup, string accessToken)
        {
            if (autoDocTag.CustomFieldGroup == null)
                return string.Empty;

            if (string.IsNullOrWhiteSpace(autoDocTag.CustomFieldGroup.FieldName) || string.IsNullOrWhiteSpace(autoDocTag.CustomFieldGroup.CustomFieldGroupName))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(autoDocTag.TaskGroup))
            {
                if (string.IsNullOrWhiteSpace(autoDocTag.TaskItem))
                    return await GetCustomFieldGroupValueForLinkId(autoDocTag, parentTaskGroup.Id, LinkTypes.TaskGroup, accessToken);

                var childTask = parentTaskGroup.ChildTasks.FirstOrDefault(c => c.ShortName == autoDocTag.TaskItem);

                if(childTask != null)
                    return await GetCustomFieldGroupValueForLinkId(autoDocTag, childTask.Id, LinkTypes.Task, accessToken);

                return string.Empty;
            }

            if(!string.IsNullOrWhiteSpace(autoDocTag.TaskItem))
            {
                if(autoDocTag.TaskGroup == parentTaskGroup.ShortName)
                {
                    var childTask = parentTaskGroup.ChildTasks.FirstOrDefault(c => c.ShortName == autoDocTag.TaskItem);

                    if (childTask != null)
                        return await GetCustomFieldGroupValueForLinkId(autoDocTag, childTask.Id, LinkTypes.Task, accessToken);

                    return string.Empty;
                }

                var childTaskGroup = parentTaskGroup.ChildTaskGroups.FirstOrDefault(c => c.ShortName == autoDocTag.TaskGroup);

                if(childTaskGroup != null)
                {
                    var childTask = childTaskGroup.ChildTasks.FirstOrDefault(c => c.ShortName == autoDocTag.TaskItem);

                    if (childTask != null)
                        return await GetCustomFieldGroupValueForLinkId(autoDocTag, childTask.Id, LinkTypes.Task, accessToken);

                    return string.Empty;
                }
            }

            if (autoDocTag.TaskGroup == parentTaskGroup.ShortName)
            {
                return await GetCustomFieldGroupValueForLinkId(autoDocTag, parentTaskGroup.Id, LinkTypes.TaskGroup, accessToken);
            }

            var childTaskGroupHead = parentTaskGroup.ChildTaskGroups.FirstOrDefault(c => c.ShortName == autoDocTag.TaskGroup);

            if (childTaskGroupHead != null)
                return await GetCustomFieldGroupValueForLinkId(autoDocTag, childTaskGroupHead.Id, LinkTypes.TaskGroup, accessToken);

            return string.Empty;
        }

        private async Task<string> GetCustomFieldGroupValueForLinkId(AutoDocTag autoDocTag, Guid linkId, string linkType, string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CUSTOMFIELD, $"CustomFieldGroupLink/GetLinks/{linkType}/{linkId}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var customFieldGroupLnkList = response.ContentAsType<CustomFieldGroupLnkList>();

                var customFieldGroupLink = customFieldGroupLnkList.CustomFieldGroupLinks.FirstOrDefault(c => c.CustomFieldGroupHeader.ShortName == autoDocTag.CustomFieldGroup.CustomFieldGroupName);

                if (customFieldGroupLink != null)
                {
                    response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CUSTOMFIELD, $"CustomFieldValue/GetValuesForLink/{linkType}/{linkId}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var customFieldValueList = response.ContentAsType<CustomFieldValList>();

                        var customFieldHeader = customFieldGroupLink.CustomFieldGroupHeader.CustomFieldHeaders.FirstOrDefault(c => c.ShortName == autoDocTag.CustomFieldGroup.FieldName);

                        if(customFieldHeader != null)
                        {
                            var customFieldValue = customFieldValueList.CustomFieldValues.FirstOrDefault(c => c.CustomFieldHeaderId == customFieldHeader.Id);

                            if (customFieldValue != null)
                            {
                                var customFieldValueMulti = new CustomFieldValMultiType()
                                {
                                    CustomFieldHeaderId = customFieldHeader.Id,
                                    Id = Guid.Empty,
                                    StringValue = customFieldValue.FieldValue,
                                    LinkId = linkId,
                                    LinkType = linkType
                                };

                                if (customFieldHeader.CustomFieldType == CustomFieldTypes.DateTimeField)
                                    return customFieldValueMulti.DateTimeValue.ToShortDateString();

                                if(customFieldHeader.CustomFieldType == CustomFieldTypes.BoolField)
                                    return customFieldValueMulti.BoolValue.ToString();

                                return customFieldValueMulti.StringValue;
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        private async Task<string> GetHeaderValue(AutoDocTag autoDocTag, TaskGroupHead parentTaskGroup, string accessToken)
        {
            if(string.IsNullOrWhiteSpace(autoDocTag.TaskGroup) && string.IsNullOrWhiteSpace(autoDocTag.TaskItem) && autoDocTag.CustomFieldGroup == null)
            {
                return await GetHeaderValueFromTaskGroup(autoDocTag, parentTaskGroup, accessToken);
            }

            if (!string.IsNullOrWhiteSpace(autoDocTag.TaskGroup) && string.IsNullOrWhiteSpace(autoDocTag.TaskItem) && autoDocTag.CustomFieldGroup == null)
            {
                if(parentTaskGroup.ShortName == autoDocTag.TaskGroup)
                    return await GetHeaderValueFromTaskGroup(autoDocTag, parentTaskGroup, accessToken);

                var childTaskGroupHead = parentTaskGroup.ChildTaskGroups.FirstOrDefault(c => c.ShortName == autoDocTag.TaskGroup);

                if(childTaskGroupHead != null)
                {
                    return await GetHeaderValueFromTaskGroup(autoDocTag, childTaskGroupHead, accessToken);
                }

                return string.Empty;
            }

            if(!string.IsNullOrWhiteSpace(autoDocTag.TaskItem) && autoDocTag.CustomFieldGroup == null)
            {
                if(string.IsNullOrWhiteSpace(autoDocTag.TaskGroup))
                {
                    var childTaskItem = parentTaskGroup.ChildTasks.FirstOrDefault(c => c.ShortName == autoDocTag.TaskItem);

                    if (childTaskItem != null)
                        return await GetHeaderValueFromTask(autoDocTag, childTaskItem, accessToken);

                    return string.Empty;
                }

                var childTaskGroup = parentTaskGroup.ChildTaskGroups.FirstOrDefault(c => c.ShortName == autoDocTag.TaskGroup);

                if(childTaskGroup != null)
                {
                    var childTaskItem = childTaskGroup.ChildTasks.FirstOrDefault(c => c.ShortName == autoDocTag.TaskItem);

                    if (childTaskItem != null)
                        return await GetHeaderValueFromTask(autoDocTag, childTaskItem, accessToken);

                    return string.Empty;
                }
            }

            return string.Empty;
        }

        private async Task<string> GetHeaderValueFromTaskGroup(AutoDocTag autoDocTag, TaskGroupHead taskGroupHead, string accessToken)
        {
            if (autoDocTag.ClientName != null || autoDocTag.MainContactName != null || autoDocTag.MainContactEmail != null || autoDocTag.MainContactPhone != null)
                return await GetClientDetail(autoDocTag, taskGroupHead.ClientHeaderId, accessToken);

            if (autoDocTag.CompletionDate != null)
                return taskGroupHead.Completed ? taskGroupHead.CompletedOnDate.ToShortDateString() : taskGroupHead.CompletionDate.ToShortDateString();

            if (autoDocTag.Description != null)
                return taskGroupHead.Description;

            if (autoDocTag.LongName != null)
                return taskGroupHead.LongName;

            if (autoDocTag.StartDate != null)
                return taskGroupHead.StartDate.ToShortDateString();

            if(autoDocTag.UserName != null)
                return await GetUserName(autoDocTag, taskGroupHead.MainUserId, accessToken);

            return string.Empty;
        }

        private async Task<string> GetHeaderValueFromTask(AutoDocTag autoDocTag, TaskHead taskHead, string accessToken)
        {
            if (autoDocTag.CompletionDate != null)
                return taskHead.Completed ? taskHead.CompletedOnDate.ToShortDateString() : taskHead.CompletionDate.ToShortDateString();

            if (autoDocTag.Description != null)
                return taskHead.Description;

            if (autoDocTag.LongName != null)
                return taskHead.Title;

            if (autoDocTag.StartDate != null)
                return taskHead.StartDate.ToShortDateString();

            if (autoDocTag.UserName != null)
                return await GetUserName(autoDocTag, taskHead.MainUserId, accessToken);

            return string.Empty;
        }

        private async Task<string> GetUserName(AutoDocTag autoDocTag, Guid UserId, string accessToken)
        {
            if (autoDocTag.UserName != null)
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.IDENTITY, $"User/{UserId}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var userModel = response.ContentAsType<UserModel>();

                    return userModel.FullName;
                }
            }

            return string.Empty;
        }

        private async Task<string> GetClientDetail(AutoDocTag autoDocTag, Guid clientHeaderId, string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CLIENT, $"Client/GetClientSummary/{clientHeaderId}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var clientSummary = response.ContentAsType<ClientSummary>();

                if (autoDocTag.ClientName != null)
                    return clientSummary.LongName;

                if (autoDocTag.MainContactName != null)
                    return clientSummary.MainContactName;

                if (autoDocTag.MainContactEmail != null)
                    return clientSummary.Email;

                if (autoDocTag.MainContactPhone != null)
                    return clientSummary.Phone;
            }

            return string.Empty;
        }

        private async Task<TaskGroupHead> GetTaskForLink(Guid linkId, string linkType, string accessToken)
        {
            Guid taskGroupHeaderId = linkId;

            if (linkType == LinkTypes.Task)
            {
                var taskHead = await GetTaskHead(linkId, accessToken);

                if(taskHead != null)
                    taskGroupHeaderId = taskHead.TaskGroupHeaderId;
            }

            return await GetParentTaskGroupHead(taskGroupHeaderId, accessToken);
        }

        private async Task<TaskHead> GetTaskHead(Guid taskHeadId, string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"Task/{taskHeadId}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ContentAsType<TaskHead>();
            }

            return null;
        }

        private async Task<TaskGroupHead> GetParentTaskGroupHead(Guid taskGroupHeadId, string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/{taskGroupHeadId}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var taskGroupHead = response.ContentAsType<TaskGroupHead>();

                if(taskGroupHead.ParentTaskGroupId.HasValue)
                    taskGroupHead = await GetParentTaskGroupHead(taskGroupHead.ParentTaskGroupId.Value, accessToken);

                return taskGroupHead;
            }

            return null;
        }

        private void FindVariablesInDocument(AutoDoc autoDoc)
        {
            var regex = new Regex($"({MakeTagRegexSafe(autoDoc.OpenVariableSymbol)})(.*?)({MakeTagRegexSafe(autoDoc.CloseVariableSymbol)})");

            using (MemoryStream mem = new MemoryStream())
            {
                mem.Write(autoDoc.Content, 0, (int)autoDoc.Content.Length);
                using (WordprocessingDocument wDoc = WordprocessingDocument.Open(mem, false))
                {
                    string docText = wDoc.MainDocumentPart.Document.Body.InnerText;
                    //using (StreamReader sr = new StreamReader(wDoc.MainDocumentPart.GetStream()))
                    //{
                    //    docText = sr.ReadToEnd();
                    //}

                    var foundMatches = regex.Matches(docText);

                    foreach (Match match in foundMatches)
                    {
                        var foundMatch = string.Empty;
                        if (match.Groups.Count >= 3)
                            foundMatch = match.Groups[2].Value;
                            //autoDoc.AutoValues.Add(new AutoVal(match.Groups[2].Value, string.Empty));
                        else
                            foundMatch = match.Value;
                        //autoDoc.AutoValues.Add(new AutoVal(match.Value, string.Empty));

                        var foundExistingValue = autoDoc.AutoValues.FirstOrDefault(a => a.Value == foundMatch);

                        if(foundExistingValue == null)
                            autoDoc.AutoValues.Add(new AutoVal(foundMatch, string.Empty));
                    }

                    //var xDoc = wDoc.MainDocumentPart.GetXDocument();

                    //var count = OpenXmlRegex.Match(xDoc.Descendants(), regex, (element, match) =>
                    //    autoDoc.AutoValues.Add(new AutoVal(match.Value, string.Empty)));
                }
            }
        }

        private string MakeTagRegexSafe(string tag)
        {
            var newTag = tag;

            newTag = newTag.Replace("$", "\\$");

            return newTag;
        }

        private byte[] ProcessDocument(byte[] documentToProcess, IList<AutoVal> values, string openVariableSymbol, string closeVariableSymbol)
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

        private async Task GetStartAndEndTags(AutoDoc autoDoc, string accessToken)
        {
            var startTagSetting = await _apiHelper.GetSystemSetting(accessToken, "DOC_AUTO_START_TAG");
            autoDoc.OpenVariableSymbol = startTagSetting.Value;
            var endTagSetting = await _apiHelper.GetSystemSetting(accessToken, "DOC_AUTO_END_TAG");
            autoDoc.CloseVariableSymbol = endTagSetting.Value;
        }
    }
}

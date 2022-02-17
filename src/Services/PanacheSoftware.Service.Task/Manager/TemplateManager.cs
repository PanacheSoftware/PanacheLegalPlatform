using AutoMapper;
using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.API.Task.Template;
using PanacheSoftware.Core.Domain.File;
using PanacheSoftware.Core.Domain.Task.Template;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Task.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Manager
{
    public class TemplateManager : ITemplateManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;
        private readonly IAPIHelper _apiHelper;
        private readonly ITaskManager _taskManager;
        private List<Guid> _userTeams;

        public TemplateManager(IUnitOfWork unitOfWork, IMapper mapper, IUserProvider userProvider, IAPIHelper apiHelper, ITaskManager taskManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
            _apiHelper = apiHelper;
            _taskManager = taskManager;   
        }

        public async Task<TemplateHeadList> GetTemplateHeadListAsync()
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

        public async Task<Tuple<Guid, string>> CreateTemplateFromTask(TemplateHead templateHead, Guid TaskGroupHeaderId, string accessToken)
        {
            var returnResult = new Tuple<Guid, string>(Guid.Empty, string.Empty);

            var taskGroupSummary = await _taskManager.GetTaskGroupSummaryAsync(TaskGroupHeaderId, accessToken);

            if (taskGroupSummary == null)
                return new Tuple<Guid, string>(Guid.Empty, $"Id: {TaskGroupHeaderId}, Is not a main task group");

            var templateHeader = _mapper.Map<TemplateHeader>(templateHead);

            templateHeader.TemplateDetail.TotalDays = (taskGroupSummary.CompletionDate - taskGroupSummary.StartDate).Days;

            //templateHeader.TemplateGroupHeaders = new List<TemplateGroupHeader>();

            _unitOfWork.TemplateHeaders.Add(templateHeader);

            _unitOfWork.Complete();

            var taskFileLinks = new List<FileHead>();
            var customFieldLinks = new List<CustomFieldGroupLnk>();

            await _taskManager.GetCustomFieldLinks(customFieldLinks, LinkTypes.TaskGroup, TaskGroupHeaderId, LinkTypes.Template, templateHeader.Id, accessToken);

            foreach (var taskGroupHeader in taskGroupSummary.ChildTaskGroups)
            {
                var templateGroupHeader = new TemplateGroupHeader();

                templateGroupHeader.Description = taskGroupHeader.Description;
                templateGroupHeader.ShortName = taskGroupHeader.ShortName;
                templateGroupHeader.LongName = taskGroupHeader.LongName;
                templateGroupHeader.SequenceNumber = taskGroupHeader.SequenceNumber;
                templateGroupHeader.Status = StatusTypes.Open;

                templateGroupHeader.TemplateGroupDetail = new TemplateGroupDetail();
                templateGroupHeader.TemplateGroupDetail.Status = StatusTypes.Open;
                templateGroupHeader.TemplateGroupDetail.DaysOffset = (taskGroupHeader.StartDate - taskGroupSummary.StartDate).Days;
                templateGroupHeader.TemplateGroupDetail.TotalDays = (taskGroupHeader.CompletionDate - taskGroupHeader.StartDate).Days;

                templateGroupHeader.TemplateHeaderId = templateHeader.Id;

                _unitOfWork.TemplateGroupHeaders.Add(templateGroupHeader);

                _unitOfWork.Complete();

                await _taskManager.GetCustomFieldLinks(customFieldLinks, LinkTypes.TaskGroup, taskGroupHeader.Id, LinkTypes.Template, templateGroupHeader.Id, accessToken);

                //templateGroupHeader.TemplateItemHeaders = new List<TemplateItemHeader>();

                foreach (var taskItemHeader in taskGroupHeader.ChildTasks)
                {
                    var templateItemHeader = new TemplateItemHeader();

                    templateItemHeader.Description = taskItemHeader.Description;
                    templateItemHeader.Status = StatusTypes.Open;
                    templateItemHeader.TaskType = NodeTypes.Task;
                    templateItemHeader.Title = taskItemHeader.Title;
                    templateItemHeader.TemplateItemDetail = new TemplateItemDetail();
                    templateItemHeader.TemplateItemDetail.Status = StatusTypes.Open;
                    templateItemHeader.SequenceNumber = taskItemHeader.SequenceNumber;
                    templateItemHeader.TemplateItemDetail.DaysOffset = (taskItemHeader.StartDate - taskGroupHeader.StartDate).Days;
                    templateItemHeader.TemplateItemDetail.TotalDays = (taskItemHeader.CompletionDate - taskItemHeader.StartDate).Days;

                    templateItemHeader.TemplateGroupHeaderId = templateGroupHeader.Id;

                    _unitOfWork.TemplateItemHeaders.Add(templateItemHeader);

                    _unitOfWork.Complete();

                    await _taskManager.GetCustomFieldLinks(customFieldLinks, LinkTypes.Task, taskItemHeader.Id, LinkTypes.Template, templateItemHeader.Id, accessToken);

                    var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FILE, $"File/Link/GetFilesForLink/{NodeTypes.Task}/{taskItemHeader.Id}");

                    if(response.IsSuccessStatusCode)
                    {
                        var fileList = response.ContentAsType<FileList>();

                        foreach (var fileHead in fileList.FileHeaders)
                        {
                            var fileHeadToCreate = new FileHead();
                            var fileVer = new FileVer();
                            var latestFileVerion = fileHead.FileVersions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();

                            if (latestFileVerion != null)
                            {
                                fileHeadToCreate.FileDetail.FileTitle = fileHead.FileDetail.FileTitle;
                                fileHeadToCreate.FileDetail.Description = fileHead.FileDetail.Description;

                                if (!string.IsNullOrWhiteSpace(latestFileVerion.URI))
                                {
                                    fileHeadToCreate.FileDetail.FileExtension = "URI";
                                    fileHeadToCreate.FileDetail.FileType = "URI";
                                    fileVer.URI = latestFileVerion.URI;
                                    fileVer.VersionNumber = 0;
                                }
                                else
                                {
                                    fileHeadToCreate.FileDetail.FileExtension = fileHead.FileDetail.FileExtension;
                                    fileHeadToCreate.FileDetail.FileType = fileHead.FileDetail.FileType;
                                    fileVer.Content = latestFileVerion.Content;
                                    fileVer.UntrustedName = latestFileVerion.UntrustedName;
                                    fileVer.UploadDate = DateTime.Today;
                                    fileVer.Size = latestFileVerion.Size;
                                    fileVer.VersionNumber = 0;
                                }

                                fileHeadToCreate.FileVersions.Add(fileVer);

                                fileHeadToCreate.FileLinks.Add(new FileLnk()
                                {
                                    LinkId = templateItemHeader.Id,
                                    LinkType = LinkTypes.Template,
                                    FileHeaderId = fileHeadToCreate.Id
                                });
                            }

                            taskFileLinks.Add(fileHeadToCreate);
                        }
                    }
                }
            }
            
            foreach (var fileToCreate in taskFileLinks)
            {
                HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(fileToCreate), Encoding.UTF8, "application/json");

                try
                {
                    var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Post, APITypes.FILE, $"File", contentPost);

                    if (response.StatusCode != System.Net.HttpStatusCode.Created)
                    {
                        return new Tuple<Guid, string>(Guid.Empty, $"Error creating file for template: {fileToCreate.FileLinks.FirstOrDefault().LinkId}");
                    }
                }
                catch (Exception ex)
                {
                    return new Tuple<Guid, string>(Guid.Empty, $"Error creating file for template: {fileToCreate.FileLinks.FirstOrDefault().LinkId}");
                }
            }

            foreach (var customFieldGroupLink in customFieldLinks)
            {
                HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(customFieldGroupLink), Encoding.UTF8, "application/json");

                try
                {
                    var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Post, APITypes.CUSTOMFIELD, $"CustomFieldGroupLink", contentPost);

                    if (response.StatusCode != System.Net.HttpStatusCode.Created)
                    {
                        return new Tuple<Guid, string>(Guid.Empty, $"Error creating custom filed group link for template: {customFieldGroupLink.LinkId}");
                    }
                }
                catch (Exception ex)
                {
                    return new Tuple<Guid, string>(Guid.Empty, $"Error creating custom filed group link for template: {customFieldGroupLink.LinkId}");
                }
            }

            return new Tuple<Guid, string>(templateHeader.Id, $"Created okay");
        }
    }
}

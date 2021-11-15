using AutoMapper;
using Newtonsoft.Json;
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

        public async Task<Tuple<Guid, string>> CreateTemplateFromTask(TemplateHead templateHead, Guid TaskGroupHeaderId, string accessToken)
        {
            var returnResult = new Tuple<Guid, string>(Guid.Empty, string.Empty);

            var taskGroupSummary = await _taskManager.GetTaskGroupSummaryAsync(TaskGroupHeaderId, accessToken);

            if (taskGroupSummary == null)
                return new Tuple<Guid, string>(Guid.Empty, $"Id: {TaskGroupHeaderId}, Is not a main task group");

            var templateHeader = _mapper.Map<TemplateHeader>(templateHead);

            templateHeader.TemplateDetail.TotalDays = (taskGroupSummary.CompletionDate - taskGroupSummary.StartDate).Days;

            templateHeader.TemplateGroupHeaders = new List<TemplateGroupHeader>();

            var taskFileLinks = new List<Tuple<int, int, FileHead>>();

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

                templateGroupHeader.TemplateItemHeaders = new List<TemplateItemHeader>();

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
                                    fileVer.Content = fileVer.Content;
                                    fileVer.UntrustedName = fileVer.UntrustedName;
                                    fileVer.UploadDate = DateTime.Today;
                                    fileVer.Size = fileVer.Size;
                                    fileVer.VersionNumber = 0;
                                }
                            }

                            taskFileLinks.Add(Tuple.Create(templateGroupHeader.SequenceNumber, templateItemHeader.SequenceNumber, fileHeadToCreate));
                        }
                    }

                    templateGroupHeader.TemplateItemHeaders.Add(templateItemHeader);
                }

                templateHeader.TemplateGroupHeaders.Add(templateGroupHeader);
            }
            
            _unitOfWork.TemplateHeaders.Add(templateHeader);

            _unitOfWork.Complete();

            foreach (var fileToCreate in taskFileLinks)
            {
                var templateGroup = templateHeader.TemplateGroupHeaders.Where(tg => tg.SequenceNumber == fileToCreate.Item1).FirstOrDefault();

                if(templateGroup != null)
                {
                    var templateItem = templateGroup.TemplateItemHeaders.Where(ti => ti.SequenceNumber == fileToCreate.Item2).FirstOrDefault();

                    if(templateItem != null)
                    {
                        var fileHeadToCreate = fileToCreate.Item3;

                        fileHeadToCreate.FileLinks.Add(new FileLnk()
                        {
                            LinkId = templateItem.Id,
                            LinkType = LinkTypes.Template,
                            FileHeaderId = fileHeadToCreate.Id
                        });

                        HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(fileHeadToCreate), Encoding.UTF8, "application/json");

                        try
                        {
                            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Post, APITypes.FILE, $"File", contentPost);

                            if (response.StatusCode != System.Net.HttpStatusCode.Created)
                            {
                                return new Tuple<Guid, string>(Guid.Empty, $"Error creating file for template: {fileToCreate.Item1}, {fileToCreate.Item2}");
                            }
                        }
                        catch (Exception ex)
                        {
                            return new Tuple<Guid, string>(Guid.Empty, $"Error creating file for template: {fileToCreate.Item1}, {fileToCreate.Item2}");
                        }
                    }
                }
            }

            return new Tuple<Guid, string>(templateHeader.Id, $"Created okay");
        }
    }
}

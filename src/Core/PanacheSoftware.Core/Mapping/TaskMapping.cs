using AutoMapper;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.Task;
using System;
using System.Linq;

namespace PanacheSoftware.Core.Mapping
{
    public class TaskMapping : Profile
    {
        public TaskMapping()
        {
            CreateMap<TaskHeader, TaskHead>();
            CreateMap<TaskDetail, TaskDet>();
            CreateMap<TaskHead, TaskHeader>();
            CreateMap<TaskDet, TaskDetail>();
            CreateMap<TaskGroupHeader, TaskGroupHead>();
            CreateMap<TaskGroupDetail, TaskGroupDet>();
            CreateMap<TaskGroupHead, TaskGroupHeader>();
            CreateMap<TaskGroupDet, TaskGroupDetail>();
            CreateMap<TaskGroupHeader, TaskGroupSummary>();
            CreateMap<TaskGroupSummary, TaskGroupHeader>();
            CreateMap<TaskHeader, TaskSummary>();
            CreateMap<TaskSummary, TaskHeader>();
        }
    }
}

using AutoMapper;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Service.Task.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Manager
{
    public class TaskManager : ITaskManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<Guid> GetChildTaskGroupIds(Guid taskGroupHeaderId)
        {
            List<Guid> childTaskGroupHeaders = new List<Guid>();

            var taskGroupTree = _unitOfWork.TaskGroupHeaders.GetTaskGroupTree(taskGroupHeaderId);

            if (taskGroupTree.Any())
            {
                var queue = new Queue<TaskGroupHeader>();
                queue.Enqueue(taskGroupTree[0]);

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();

                    foreach (var childTaskGroupHeader in node.ChildTaskGroups)
                    {
                        if (!childTaskGroupHeaders.Contains(childTaskGroupHeader.Id))
                            childTaskGroupHeaders.Add(childTaskGroupHeader.Id);

                        queue.Enqueue(childTaskGroupHeader);
                    }
                }
            }

            return childTaskGroupHeaders;
        }

        public TaskGroupList GetMainTaskGroups()
        {
            TaskGroupList taskGroupList = new TaskGroupList();

            foreach (var currentTaskGroupHeader in _unitOfWork.TaskGroupHeaders.GetMainTaskGroups(false))
            {
                taskGroupList.TaskGroupHeaders.Add(_mapper.Map<TaskGroupHead>(currentTaskGroupHeader));
            }

            return taskGroupList;
        }

        public TaskGroupSummaryList GetMainTaskGroupSummarys()
        {
            TaskGroupSummaryList taskGroupSummaryList = new TaskGroupSummaryList();

            foreach (var currentTaskGroupHeader in _unitOfWork.TaskGroupHeaders.GetMainTaskGroups(true))
            {
                taskGroupSummaryList.TaskGroupSummarys.Add(_mapper.Map<TaskGroupSummary>(currentTaskGroupHeader));
            }

            return taskGroupSummaryList;
        }

        public TaskGroupSummary GetTaskGroupSummary(Guid taskGroupHeaderId)
        {
            var taskGroupHeader = _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelations(taskGroupHeaderId, true);

            if(taskGroupHeader != null)
            {
                if(taskGroupHeader.ParentTaskGroupId == null)
                {
                    return _mapper.Map<TaskGroupSummary>(taskGroupHeader);
                }
            }

            return null;
        }

        public TaskGroupList GetTaskGroupList(Guid taskGroupHeaderId = default, bool validParents = false)
        {
            TaskGroupList taskGroupList = new TaskGroupList();

            if (taskGroupHeaderId == Guid.Empty)
            {
                foreach (var currentTaskGroupHeader in _unitOfWork.TaskGroupHeaders.GetAll(true))
                {
                    taskGroupList.TaskGroupHeaders.Add(_mapper.Map<TaskGroupHead>(currentTaskGroupHeader));
                }
            }
            else
            {
                var childFolders = new List<Guid>();

                //Check if we only want to return teams that would be applicable as a parent for the passed in TeamHeader Id (i.e. aren't already a child of the TeamHeader passed in)
                if (validParents)
                {
                    childFolders = GetChildTaskGroupIds(taskGroupHeaderId);
                    childFolders.Add(taskGroupHeaderId);
                }

                foreach (var currentTaskGroupHeader in _unitOfWork.TaskGroupHeaders.GetAll(true))
                {
                    if (!childFolders.Contains(currentTaskGroupHeader.Id))
                        taskGroupList.TaskGroupHeaders.Add(_mapper.Map<TaskGroupHead>(currentTaskGroupHeader));
                }
            }

            return taskGroupList;
        }

        

        public bool SetNewTaskGroupSequenceNo(TaskGroupHeader taskGroupHeader)
        {
            taskGroupHeader.SequenceNumber = 0;

            Guid parentTaskGroupId = taskGroupHeader.ParentTaskGroupId ?? Guid.Empty;
            if (parentTaskGroupId != Guid.Empty)
            {
                TaskGroupHeader parentTaskGroupHeader = _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelations(parentTaskGroupId, true);

                if (parentTaskGroupHeader != null)
                {
                    var maxChildTaskGroup = parentTaskGroupHeader.ChildTaskGroups.OrderByDescending(f => f.SequenceNumber).FirstOrDefault();

                    if (maxChildTaskGroup != null)
                    {
                        taskGroupHeader.SequenceNumber = maxChildTaskGroup.SequenceNumber + 1;
                    }
                }
            }

            return true;
        }

        public bool SetNewTaskSequenceNo(TaskHeader taskHeader)
        {
            taskHeader.SequenceNumber = 0;

            Guid parentTaskGroupHeaderId = taskHeader.TaskGroupHeaderId;
            if (parentTaskGroupHeaderId != Guid.Empty)
            {
                TaskGroupHeader parentTaskGroupHeader = _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelations(parentTaskGroupHeaderId, true);

                if (parentTaskGroupHeader != null)
                {
                    var maxChildTask = parentTaskGroupHeader.ChildTasks.OrderByDescending(f => f.SequenceNumber).FirstOrDefault();

                    if (maxChildTask != null)
                    {
                        taskHeader.SequenceNumber = maxChildTask.SequenceNumber + 1;
                    }
                }
            }

            return true;
        }

        public bool TaskGroupParentOkay(TaskGroupHeader taskGroupHeader)
        {
            Guid parentTaskGroupId = taskGroupHeader.ParentTaskGroupId ?? Guid.Empty;
            if (parentTaskGroupId != Guid.Empty)
            {
                TaskGroupHeader parentTaskGroupHeader = _unitOfWork.TaskGroupHeaders.Get(parentTaskGroupId);

                if (parentTaskGroupHeader == null)
                {
                    return false;
                }

                taskGroupHeader.ClientHeaderId = Guid.Empty;
                taskGroupHeader.TeamHeaderId = Guid.Empty;
                //taskGroupHeader.MainUserId = Guid.Empty;
            }
            else
            {
                taskGroupHeader.ParentTaskGroupId = null;
            }

            return true;
        }
    }
}

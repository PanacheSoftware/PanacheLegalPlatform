using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Core.Helpers
{
    public static class GanttHelper
    {
        public static GanttDataModel GenerateGanttDataModel(TaskGroupSummary taskGroupSummary)
        {
            var ganttDataModel = new GanttDataModel();

            var ganttData = new List<GanttData>();
            var ganttList = new List<GanttLink>();

            AddToGanttDataList(ganttData, taskGroupSummary, null);

            ganttDataModel.Data = ganttData.ToArray();
            ganttDataModel.Links = ganttList.ToArray();

            return ganttDataModel;
        }

        private static void AddToGanttDataList(List<GanttData> ganttData, TaskGroupSummary taskGroupSummary, Guid? parentId)
        {
            ganttData.Add(new GanttData()
            {
                Id = taskGroupSummary.Id.ToString(),
                Text = taskGroupSummary.LongName,
                StartDate = (!taskGroupSummary.ChildTaskGroups.Any() && !taskGroupSummary.ChildTasks.Any()) ? taskGroupSummary.StartDate.ToString("yyyy-MM-dd HH:mm") : null,
                Duration = (!taskGroupSummary.ChildTaskGroups.Any() && !taskGroupSummary.ChildTasks.Any()) ? Convert.ToInt64((taskGroupSummary.CompletionDate - taskGroupSummary.StartDate).TotalDays) : 0,
                Parent = parentId != null ? parentId.ToString() : string.Empty,
                Progress = taskGroupSummary.PercentageComplete,
                Open = true
            });

            foreach (var childTask in taskGroupSummary.ChildTasks)
            {
                ganttData.Add(new GanttData()
                {
                    Id = childTask.Id.ToString(),
                    Text = childTask.Title,
                    StartDate = childTask.StartDate.ToString("yyyy-MM-dd HH:mm"),
                    Duration = childTask.Completed ? Convert.ToInt64((childTask.CompletedOnDate - childTask.StartDate).TotalDays)  : Convert.ToInt64((childTask.CompletionDate - childTask.StartDate).TotalDays),
                    Parent = childTask.TaskGroupHeaderId.ToString(),
                    Progress = childTask.Completed ? 1.0 : 0
                });
            }

            foreach (var childTaskGroup in taskGroupSummary.ChildTaskGroups)
            {
                AddToGanttDataList(ganttData, childTaskGroup, taskGroupSummary.Id);
            }
        }
    }
}

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
        public static PLGanttModel GenerateChartJSGanttModel(TaskGroupSummary taskGroupSummary)
        {
            var plGanttModel = new PLGanttModel();

            AddGanttEntry(plGanttModel, taskGroupSummary);

            return plGanttModel;
        }

        private static void AddGanttEntry(PLGanttModel plGanttModel, TaskGroupSummary taskGroupSummary)
        {
            var ganttDataHolder = new GanttDataHolder(true, 
                taskGroupSummary.StartDate,
                taskGroupSummary.CompletedOnDate == DateTime.Parse("01/01/1900") ? taskGroupSummary.CompletionDate : taskGroupSummary.CompletedOnDate, 
                MakeStringJSONSafe(taskGroupSummary.LongName),
                taskGroupSummary.Completed);

            plGanttModel.AddDataHolder(ganttDataHolder);

            foreach (var childTask in taskGroupSummary.ChildTasks)
            {
                var childTaskGanttDataHolder = new GanttDataHolder(false,
                childTask.StartDate,
                childTask.CompletedOnDate == DateTime.Parse("01/01/1900") ? childTask.CompletionDate : childTask.CompletedOnDate,
                MakeStringJSONSafe(childTask.Title),
                childTask.Completed);

                plGanttModel.AddDataHolder(childTaskGanttDataHolder);
            }

            foreach (var childTaskGroup in taskGroupSummary.ChildTaskGroups)
            {
                AddGanttEntry(plGanttModel, childTaskGroup);
            }
        }

        public static GCGanttModel GenerateGoogleChartsGanttModel(TaskGroupSummary taskGroupSummary)
        {
            var gcGanttModel = new GCGanttModel();

            gcGanttModel.Columns = GenerateGoogleChartsGanttColumns().ToArray();

            var ganttRows = new List<GCGanttDataRow>();

            AddGoogleChartsGanttDataRows(ganttRows, taskGroupSummary, null);

            gcGanttModel.Rows = ganttRows.ToArray();

            return gcGanttModel;
        }

        private static IList<GCGanttColumn> GenerateGoogleChartsGanttColumns()
        {
            var gcGanttColumns = new List<GCGanttColumn>();

            gcGanttColumns.Add(new GCGanttColumn() { Label = "Task ID", Type = "string" });
            gcGanttColumns.Add(new GCGanttColumn() { Label = "Task Name", Type = "string" });
            gcGanttColumns.Add(new GCGanttColumn() { Label = "Resource", Type = "string" });
            gcGanttColumns.Add(new GCGanttColumn() { Label = "Start Date", Type = "date" });
            gcGanttColumns.Add(new GCGanttColumn() { Label = "End Date", Type = "date" });
            gcGanttColumns.Add(new GCGanttColumn() { Label = "Duration", Type = "number" });
            gcGanttColumns.Add(new GCGanttColumn() { Label = "Percent Complete", Type = "number" });
            gcGanttColumns.Add(new GCGanttColumn() { Label = "Dependencies", Type = "string" });

            return gcGanttColumns;
        }

        private static void AddGoogleChartsGanttDataRows(List<GCGanttDataRow> ganttRows, TaskGroupSummary taskGroupSummary, Guid? parentId)
        {
            var gcGanttColumns = new List<GCGanttDataColumn>();

            //Resources, make sure the resource is set to the TaskID, TaskID should be made unique within a Task.
            gcGanttColumns.Add(new GCGanttDataColumn() { Value = taskGroupSummary.Id.ToString() });
            gcGanttColumns.Add(new GCGanttDataColumn() { Value = MakeStringJSONSafe(taskGroupSummary.LongName) });
            gcGanttColumns.Add(new GCGanttDataColumn() { Value = parentId.HasValue ? parentId.Value.ToString() : taskGroupSummary.Id.ToString() });
            gcGanttColumns.Add(new GCGanttDataColumn() { Value = $"Date({taskGroupSummary.StartDate.Year}, {taskGroupSummary.StartDate.Month}, {taskGroupSummary.StartDate.Day})" });
            gcGanttColumns.Add(new GCGanttDataColumn() { Value = $"Date({taskGroupSummary.CompletionDate.Year}, {taskGroupSummary.CompletionDate.Month}, {taskGroupSummary.CompletionDate.Day})" });
            gcGanttColumns.Add(new GCGanttDataColumn() { Value = null });
            gcGanttColumns.Add(new GCGanttDataColumn() { Value = Convert.ToInt32(taskGroupSummary.PercentageComplete).ToString() });
            gcGanttColumns.Add(new GCGanttDataColumn() { Value = null });

            ganttRows.Add(new GCGanttDataRow() { ColumnValues = gcGanttColumns.ToArray() });

            foreach (var childTask in taskGroupSummary.ChildTasks)
            {
                gcGanttColumns.Clear();
                gcGanttColumns.Add(new GCGanttDataColumn() { Value = childTask.Id.ToString() });
                gcGanttColumns.Add(new GCGanttDataColumn() { Value = MakeStringJSONSafe(childTask.Title) });
                gcGanttColumns.Add(new GCGanttDataColumn() { Value = childTask.TaskGroupHeaderId.ToString() });
                gcGanttColumns.Add(new GCGanttDataColumn() { Value = $"Date({childTask.StartDate.Year}, {childTask.StartDate.Month}, {childTask.StartDate.Day})" });
                gcGanttColumns.Add(new GCGanttDataColumn() { Value = $"Date({childTask.CompletionDate.Year}, {childTask.CompletionDate.Month}, {childTask.CompletionDate.Day})" });
                gcGanttColumns.Add(new GCGanttDataColumn() { Value = null });
                gcGanttColumns.Add(new GCGanttDataColumn() { Value = childTask.Completed ? "100" : "0" });
                gcGanttColumns.Add(new GCGanttDataColumn() { Value = null });

                ganttRows.Add(new GCGanttDataRow() { ColumnValues = gcGanttColumns.ToArray() });
            }

            foreach (var childTaskGroup in taskGroupSummary.ChildTaskGroups)
            {
                AddGoogleChartsGanttDataRows(ganttRows, childTaskGroup, taskGroupSummary.Id);
            }
        }

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
                Text = MakeStringJSONSafe(taskGroupSummary.LongName),
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
                    Text = MakeStringJSONSafe(childTask.Title),
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

        private static string MakeStringJSONSafe(string textString)
        {
            StringBuilder sb = new StringBuilder(textString);

            sb.Replace("\'", " ");
            sb.Replace("\"", " ");
            sb.Replace("\\", " ");
            sb.Replace("\n", " ");
            sb.Replace("\r", " ");
            sb.Replace("\t", " ");
            sb.Replace("\b", " ");
            sb.Replace("\f", " ");
            sb.Replace("\v", " ");
            sb.Replace("\0", " ");

            return sb.ToString();
        }
    }
}

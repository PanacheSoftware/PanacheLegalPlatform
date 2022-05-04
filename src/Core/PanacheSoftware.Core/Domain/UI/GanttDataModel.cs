using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;

namespace PanacheSoftware.Core.Domain.UI
{
    public class PLGanttModel
    {
        public List<long[]> DateRanges { get; set; }
        public List<string> TaskNames { get; set; }
        public List<string> TaskColours { get; set; }
        public List<string> TaskBorderColours { get; set; }
        public long StartTime { get => GetFirstTime(); }
        public long EndTime { get => GetLastTime(); }

        public PLGanttModel()
        {
            DateRanges = new List<long[]>();
            TaskNames = new List<string>();
            TaskColours = new List<string>();
            TaskBorderColours = new List<string>();
        }

        private long GetFirstTime()
        {
            if(DateRanges.Any())
                return DateRanges.Min(d => d[0]);

            return -1;
        }

        private long GetLastTime()
        {
            if (DateRanges.Any())
                return DateRanges.Max(d => d[1]);

            return -1;
        }

        public void AddDataHolder(GanttDataHolder ganttDataHolder)
        {
            if (ganttDataHolder == default)
                return;

            DateRanges.Add(new long[] { ganttDataHolder.StartTime, ganttDataHolder.EndTime });
            TaskNames.Add(ganttDataHolder.TaskName);
            TaskColours.Add(ganttDataHolder.Colour);
            TaskBorderColours.Add(ganttDataHolder.Border);
        }
    }

    public class GanttDataHolder
    {
        public bool TaskGroup { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string TaskName { get; private set; }
        public string Colour { get => GetColour(); }
        public string Border { get => GetBorder(); }
        public long StartTime { get => GetTime(StartDate); }
        public long EndTime { get => GetTime(EndDate); }
        public bool Completed { get; private set; }

        public GanttDataHolder(bool taskGroup, DateTime startDate, DateTime endDate, string taskName, bool completed)
        {
            TaskGroup = taskGroup;
            StartDate = startDate;
            EndDate = endDate;
            TaskName = taskName;
            Completed = completed;
        }

        private string GetColour()
        {
            if(Completed)
                return TaskGroup ? "rgb(255, 99, 132)" : "rgb(54, 162, 235)";

            return TaskGroup ? "rgba(255, 99, 132, 0.2)" : "rgba(54, 162, 235, 0.2)";
        }

        private string GetBorder()
        {
            return TaskGroup ? "rgb(255, 99, 132)" : "rgb(54, 162, 235)";
        }

        private long GetTime(DateTime dateTime)
        {
            var time = (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1));
            return (long)(time.TotalMilliseconds + 0.5);
        }
    }

    public partial class GCGanttModel
    {
        [JsonPropertyName("cols")]
        public GCGanttColumn[] Columns { get; set; }

        [JsonPropertyName("rows")]
        public GCGanttDataRow[] Rows { get; set; }
    }

    public partial class GCGanttDataRow
    {
        [JsonPropertyName("c")]
        public GCGanttDataColumn[] ColumnValues { get; set; }
        //data.addColumn('string', 'Task ID');
        //data.addColumn('string', 'Task Name');
        //data.addColumn('date', 'Start Date');
        //data.addColumn('date', 'End Date');
        //data.addColumn('number', 'Duration');
        //data.addColumn('number', 'Percent Complete');
        //data.addColumn('string', 'Dependencies');
    }

    public partial class GCGanttDataColumn
    {
        [JsonPropertyName("v")]
        public string Value { get; set; }
        //data.addColumn('string', 'Task ID');
        //data.addColumn('string', 'Task Name');
        //data.addColumn('date', 'Start Date');
        //data.addColumn('date', 'End Date');
        //data.addColumn('number', 'Duration');
        //data.addColumn('number', 'Percent Complete');
        //data.addColumn('string', 'Dependencies');
    }

    public partial class GCGanttColumn
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
        //data.addColumn('string', 'Task ID');
        //data.addColumn('string', 'Task Name');
        //data.addColumn('date', 'Start Date');
        //data.addColumn('date', 'End Date');
        //data.addColumn('number', 'Duration');
        //data.addColumn('number', 'Percent Complete');
        //data.addColumn('string', 'Dependencies');
    }

    public partial class GanttDataModel
    {
        [JsonPropertyName("data")]
        public GanttData[] Data { get; set; }

        [JsonPropertyName("links")]
        public GanttLink[] Links { get; set; }
    }

    public partial class GanttData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("duration")]
        public long? Duration { get; set; }

        [JsonPropertyName("parent")]
        public string Parent { get; set; }

        [JsonPropertyName("progress")]
        public double Progress { get; set; }

        [JsonPropertyName("open")]
        public bool? Open { get; set; }
    }

    public partial class GanttLink
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}

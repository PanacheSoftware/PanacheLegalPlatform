using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text.Json.Serialization;

namespace PanacheSoftware.Core.Domain.UI
{
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

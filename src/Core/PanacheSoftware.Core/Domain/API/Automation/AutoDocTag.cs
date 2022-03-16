using System.Text.Json.Serialization;

namespace PanacheSoftware.Core.Domain.API.Automation
{
    public class AutoDocTag
    {
        public AutoDocTag()
        {
            TaskGroup = null;
            TaskItem = null;
            CustomFieldGroup = null;
            ClientName = null;
            UserName = null;
            StartDate = null;
            CompletionDate = null;
            Description = null;
            LongName = null;
            MainContactName = null;
            MainContactPhone = null;
            MainContactEmail = null;
        }

        #nullable enable
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TaskGroup { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TaskItem { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CustFieldGrp? CustomFieldGroup { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ClientName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? UserName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? StartDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? CompletionDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? LongName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? MainContactName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? MainContactPhone { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? MainContactEmail { get; set; }
        #nullable disable
    }

    public class CustFieldGrp
    {
        public CustFieldGrp(string customFieldGroupName, string fieldName)
        {
            CustomFieldGroupName = customFieldGroupName;
            FieldName = fieldName;
        }

        public string CustomFieldGroupName { get; set; }

        public string FieldName { get; set; }
    }
}

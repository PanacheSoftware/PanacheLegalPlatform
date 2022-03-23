using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.CustomField
{
    public class CustomFieldVal
    {
        public CustomFieldVal()
        {
            CustomFieldValueHistorys = new List<CustomFieldValHistr>();
            LinkType = LinkTypes.Task;
            FieldValue = string.Empty;
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid CustomFieldHeaderId { get; set; }

        public Guid LinkId { get; set; }
        public string LinkType { get; set; }

        public string FieldValue { get; set; }
        public string Status { get; set; }

        public string ShortName { get; set; }

        public List<CustomFieldValHistr> CustomFieldValueHistorys { get; set; }

        //public string StringValue { get => GetStringValue(); set => SetStringValue(value); }
        //public int IntValue { get => GetIntValue(); set => SetIntValue(value); }
        //public double DoubleValue { get => GetDoubleValue(); set => SetDoubleValue(value); }
        //public DateTime DateTimeValue { get => GetDateValue(); set => SetDateValue(value); }
        //public bool BoolValue { get => GetBoolValue(); set => SetBoolValue(value); }

        private string GetStringValue()
        {
            return FieldValue;
        }

        private void SetStringValue(string value)
        {
            FieldValue = value;
        }

        private int GetIntValue()
        {
            if (int.TryParse(FieldValue, out int value))
                return value;

            return 0;
        }

        private void SetIntValue(int value)
        {
            FieldValue = value.ToString();
        }

        private double GetDoubleValue()
        {
            if (double.TryParse(FieldValue, out double value))
                return value;

            return 0D;
        }

        private void SetDoubleValue(double value)
        {
            FieldValue = value.ToString();
        }

        private DateTime GetDateValue()
        {
            var dateTimeFormatString = "yyyyMMddHHmmss";
            if (DateTime.TryParseExact(string.IsNullOrWhiteSpace(FieldValue) ? "19000101000000" : FieldValue, dateTimeFormatString, null, DateTimeStyles.None, out DateTime convertedDateTime))
                return convertedDateTime;

            return DateTime.ParseExact("19000101000000", dateTimeFormatString, null);
        }

        private void SetDateValue(DateTime value)
        {
            FieldValue = value.ToString("yyyyMMddHHmmss");
        }

        private bool GetBoolValue()
        {
            if (bool.TryParse(FieldValue, out bool value))
                return value;

            return false;
        }

        private void SetBoolValue(bool value)
        {
            FieldValue = value.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PanacheSoftware.Core.Domain.API.Error
{
    public class APIErrorMessage
    {
        public APIErrorMessage(int statusCode, string messageString, object errorModelState = null)
        {
            Status = statusCode;
            Message = messageString;
            ErrorList = new APIErrorList(null);


            if (errorModelState != null)
            {
                if (errorModelState.GetType() == typeof(ModelStateDictionary))
                {
                    ErrorList = new APIErrorList((ModelStateDictionary)errorModelState);
                }
            }
        }

        public int Status { get; set; }
        public string Message { get; set; }
        public APIErrorList ErrorList { get; set; }
    }

    public class APIErrorList
    {

        public APIErrorList(ModelStateDictionary modelStateDictionary)
        {
            APIErrors = new List<APIError>();

            if (modelStateDictionary != null)
            {
                foreach (var modelStateKey in modelStateDictionary.Keys)
                {
                    var modelStateVal = modelStateDictionary[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        APIErrors.Add(new APIError(modelStateKey, error.ErrorMessage));
                    }
                }
            }
        }

        public List<APIError> APIErrors { get; set; }
    }

    public class APIError
    {
        public APIError(string key, string message)
        {
            Key = key;
            Message = message;
        }

        public string Key { get; set; }
        public string Message { get; set; }
    }
}

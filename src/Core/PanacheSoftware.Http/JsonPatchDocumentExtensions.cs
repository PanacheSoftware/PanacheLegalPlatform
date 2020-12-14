using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper.Configuration.Conventions;

namespace PanacheSoftware.Http
{
    public static class JsonPatchDocumentExtensions
    {
        /// <summary>
        /// Adds patches based on the comparison of two objects.  Will not compare datetime types or child objects
        /// </summary>
        /// <param name="jsonPatchDocument">The JsonPatchDocument</param>
        /// <param name="originalObject">The original object</param>
        /// <param name="updatedObject">The updated object</param>
        /// <returns></returns>
        public static bool GeneratePatch(this JsonPatchDocument jsonPatchDocument, object originalObject, object updatedObject)
        {
            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.MaxDifferences = 100;
            //compareLogic.Config.TypesToIgnore = new List<Type>() { typeof(DateTime) };
            compareLogic.Config.CompareChildren = false;

            ComparisonResult comparisonResult = compareLogic.Compare(originalObject, updatedObject);

            if (!comparisonResult.AreEqual)
            {
                foreach (var difference in comparisonResult.Differences)
                {
                    if (difference.Object2TypeName == "DateTime")
                    {
                        var test = JsonSerializer.Serialize(DateTime.Parse(difference.Object2Value));
                    }

                    jsonPatchDocument.Add($"/{difference.PropertyName}",
                        difference.Object2TypeName == "DateTime"
                            ? JsonSerializer.Serialize(DateTime.Parse(difference.Object2Value)).Replace("\"", "")
                            : difference.Object2Value);
                }
                return true;
            }

            return false;
        }
    }
}

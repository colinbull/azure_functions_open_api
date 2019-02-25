using System;

namespace Inno.AzureFunctionsV2.OpenApi
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ReturnsAttribute : Attribute 
    {
        public ReturnsAttribute(int statusCode, string description = null, Type bodyType = null)
        {
            StatusCode = statusCode;
            Description = description;
            BodyType = bodyType;
        }

        public int StatusCode { get; }
        public string Description { get; }
        public Type BodyType { get; }
    }

}
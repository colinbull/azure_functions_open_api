using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.ComponentModel;

namespace Inno.AzureFunctionsV2.OpenApi.Tests
{
    public class TestElementType {
        public string Id { get; set; }
    }

    public class TestType
    {
        public string MyString { get; set; }
        public DateTime MyDateTime { get; set; }
        public double MyDouble { get; set; }
        public float MyFloat { get; set; }
        public IEnumerable<TestElementType> MyEnumerable { get; set; }

    }

    public static class MyTestFunction
    {
        [FunctionName("TestHttpTrigger")]
        [Returns((int)HttpStatusCode.OK, "OK", typeof(TestType))]
        [Returns((int)HttpStatusCode.BadRequest, "Bad", typeof(string))]
        [Returns((int)HttpStatusCode.NotFound, "NotFound")]
        [Returns((int)HttpStatusCode.Unauthorized, "Unauthorized")]
        [Returns((int)HttpStatusCode.Forbidden, "Forbidden")]
        [Description("My example test function")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "result/{sessionId}")]HttpRequest req,
            string sessionId,
            ILogger log)
        {
            return await Task.FromResult(new HttpResponseMessage());
        }
    }
}
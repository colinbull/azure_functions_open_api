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
using System.IO;
using Microsoft.OpenApi.Writers;

namespace Inno.AzureFunctionsV2.OpenApi.Tests
{

    public class OpenApiDocumentGenerationTests
    {
        [Fact]
        public void Can_Correctly_Generate_A_Schema() {
            
            var generator = new OpenApiDocumentGenerator();

            var doc = generator.GenerateOpenApiDocument(new ApiMetadata {
                Title = "Test Api",
                Description = "My Test Api",
                Version = "1.0.0.0",
                Url = "http://localhost:7071/api/"
            }, typeof(MyTestFunction).Assembly, new HashSet<Type> { 
                typeof(ILogger), 
                typeof(HttpRequest), 
                typeof(HttpTriggerAttribute) 
            });

            using(var fs = File.OpenWrite(Path.Combine(Directory.GetCurrentDirectory(), "test_openapi.json")))
            using(var sw = new StreamWriter(fs))
            {
                doc.SerializeAsV2(new OpenApiJsonWriter(sw));
            }

            // Assert.NotEmpty(funcs);

            // var func = funcs.Single();

            // Assert.Equal("TestHttpTrigger", func.Name);
            // Assert.Equal(OperationType.Get, func.Methods.Single());
            // Assert.Equal(5, func.Returns.Length);
            // Assert.Collection(func.Returns, new Action<ReturnsAttribute>[] {
            //     a => { Assert.Equal(200, a.StatusCode); Assert.Equal("OK", a.Description); Assert.Equal(typeof(TestType), a.BodyType); },
            //     a => { Assert.Equal(400, a.StatusCode); Assert.Equal("Bad", a.Description); },
            //     a => { Assert.Equal(404, a.StatusCode); Assert.Equal("NotFound", a.Description); },
            //     a => { Assert.Equal(401, a.StatusCode); Assert.Equal("Unauthorized", a.Description); },
            //     a => { Assert.Equal(403, a.StatusCode); Assert.Equal("Forbidden", a.Description); },
            // });
            // Assert.Equal("/api/result/{sessionId}", func.Route);
            // Assert.Single(func.Parameters);
            // Assert.Collection(func.Parameters, new Action<ParameterInfo>[] {
            //     p => { Assert.Equal("sessionId", p.Name); Assert.Equal(typeof(String), p.ParameterType); }
            // });

        }
    }
}
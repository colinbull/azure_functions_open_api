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

namespace Inno.AzureFunctionsV2.OpenApi.Tests
{
    
    public class FunctionResolutionTests
    {
        [Fact]
        public void Can_Correctly_Resolve_A_Function() {
            
            var funcs = FunctionResolver.GetFunctions(typeof(MyTestFunction).Assembly, new HashSet<Type> { 
                typeof(ILogger), 
                typeof(HttpRequest), 
                typeof(HttpTriggerAttribute) 
            });

            Assert.NotEmpty(funcs);

            var func = funcs.Single();

            Assert.Equal("TestHttpTrigger", func.Name);
            Assert.Equal(OperationType.Get, func.Methods.Single());
            Assert.Equal(5, func.Returns.Length);
            Assert.Collection(func.Returns, new Action<ReturnsAttribute>[] {
                a => { Assert.Equal(200, a.StatusCode); Assert.Equal("OK", a.Description); Assert.Equal(typeof(TestType), a.BodyType); },
                a => { Assert.Equal(400, a.StatusCode); Assert.Equal("Bad", a.Description); },
                a => { Assert.Equal(404, a.StatusCode); Assert.Equal("NotFound", a.Description); },
                a => { Assert.Equal(401, a.StatusCode); Assert.Equal("Unauthorized", a.Description); },
                a => { Assert.Equal(403, a.StatusCode); Assert.Equal("Forbidden", a.Description); },
            });
            Assert.Equal("/result/{sessionId}", func.Route);
            Assert.Single(func.Parameters);
            Assert.Collection(func.Parameters, new Action<ParameterInfo>[] {
                p => { Assert.Equal("sessionId", p.Name); Assert.Equal(typeof(String), p.ParameterType); }
            });

        }
    }
}
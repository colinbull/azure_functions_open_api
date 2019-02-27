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

namespace Inno.AzureFunctionsV2.OpenApi.Tests
{
    public class OpenApiSchemaGenerationTests
    {
        [Fact]
        public void Can_Walk_A_Type_Correctly_To_Generate_A_OpenApiSchema()
        {
            var schemas = typeof(TestType).GetSchemas();
            
            Assert.Equal(2, schemas.Count);
            Assert.True(schemas.ContainsKey(typeof(TestElementType).Name));
            Assert.True(schemas.ContainsKey(typeof(TestType).Name));

            Assert.Collection(schemas[typeof(TestType).Name].Properties.Keys.OrderBy(k => k), new Action<string>[] {
                (k) => Assert.Equal("MyDateTime", k),
                (k) => Assert.Equal("MyDouble", k),
                (k) => Assert.Equal("MyEnumerable", k), 
                (k) => Assert.Equal("MyFloat", k),
                (k) => Assert.Equal("MyString", k)
            });

            Assert.Collection(schemas[typeof
            (TestElementType).Name].Properties.Keys.OrderBy(k => k), new Action<string>[] {
                (k) => Assert.Equal("Id", k)
            });
        }
    }
}

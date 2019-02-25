using System;
using System.Collections.Generic;
using Xunit;

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

    public class OpenApiSchemaGenerationTests
    {
        [Fact]
        public void Can_Walk_A_Type_Correctly_To_Generate_A_OpenApiSchema()
        {
            var schemas = typeof(TestType).GetSchemas();
            
            Assert.Equal(2, schemas.Count);
            Assert.True(schemas.ContainsKey(typeof(TestElementType).Name));
            Assert.True(schemas.ContainsKey(typeof(TestType).Name));
        }
    }
}

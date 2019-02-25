using System;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Inno.AzureFunctionsV2.OpenApi
{
    public class FunctionDefinition
    {
        public string Name { get; set; }
        public Type ContainingType { get; set; }
        public ReturnsAttribute[] Returns { get; set; } = {};
        public OperationType[] Methods { get; set; } = Enum.GetValues(typeof(OperationType)).Cast<OperationType>().ToArray();
        public string Route { get; set; }
        public ParameterInfo[] Parameters { get; set; } = {};
        public string Description { get; set; }
    }
}
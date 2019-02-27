using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.OpenApi.Models;

namespace Inno.AzureFunctionsV2.OpenApi
{
    public static class FunctionResolver 
    {
        private static bool TryGetFunctionDefinition(MethodInfo methodInfo, HashSet<Type> excludedParameterTypes, out FunctionDefinition definition)
        {
            var functionAttr = methodInfo.GetAttributes<FunctionNameAttribute>().FirstOrDefault();
            var triggerAttr = methodInfo.GetParameters().SelectMany(p => p.GetCustomAttributes().OfType<HttpTriggerAttribute>()).SingleOrDefault();
            
            if(functionAttr != null && triggerAttr != null) {
               definition = new FunctionDefinition {
                    Name = functionAttr.Name,
                    ContainingType = methodInfo.DeclaringType,
                    Methods = triggerAttr.Methods.Select(m => (OperationType)Enum.Parse(typeof(OperationType),m, true)).ToArray(),
                    Route = "/" + triggerAttr.Route.TrimStart('/'),
                    Returns = methodInfo.GetAttributes<ReturnsAttribute>().ToArray(),
                    Parameters = methodInfo.GetParametersExcluding(excludedParameterTypes).ToArray()
               };

               return true;

            }

            definition = null;
            return false;
        }

        public static IEnumerable<FunctionDefinition> GetFunctions(Assembly assembly, HashSet<Type> excludedParameterTypes = null) {

            return assembly.GetTypes()
                .SelectMany(t => t.GetMethods().Aggregate(new List<FunctionDefinition>(), (funcs,methodInfo) => {
                    if(TryGetFunctionDefinition(methodInfo, excludedParameterTypes, out var def)) {
                        funcs.Add(def);
                    }

                    return funcs;
                }));      
        }
    }
}
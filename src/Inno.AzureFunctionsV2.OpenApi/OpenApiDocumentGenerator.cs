using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Inno.AzureFunctionsV2.OpenApi
{
    public class ApiMetadata
    {
        public string Title { get; set; } = "My Api";
        public string Description { get; set;} = "My Api Description";
        public string Version { get; set; } = "v1";
        public string Url { get; set; } = "/api/";

    }

    public interface IOpenApiDocumentGenerator<T>
    {
        T GenerateOpenApiDocument(ApiMetadata metaData, Assembly assembly, IEnumerable<Type> additionalParamterExclusionTypes = null);
    }

    public class OpenApiDocumentGenerator : IOpenApiDocumentGenerator<OpenApiDocument>
    {
        private static HashSet<Type> ExcludedParameterTypes = new HashSet<Type> {
            typeof(HttpRequest),
            typeof(ILogger),
            typeof(HttpTriggerAttribute)
        };

        public OpenApiDocument GenerateOpenApiDocument(ApiMetadata metaData, Assembly assembly, IEnumerable<Type> additionalParamterExclusionTypes = null)
        {
            var doc = new OpenApiDocument {
                Info = new OpenApiInfo {
                    Title = metaData.Title,
                    Description = metaData.Description,
                    Version = metaData.Version
                },
                Servers = new List<OpenApiServer> {
                    new OpenApiServer {
                        Url = metaData.Url
                    }
                },
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents {
                    Schemas = new Dictionary<string, OpenApiSchema>()
                }
            };
            
            ExcludedParameterTypes.UnionWith(additionalParamterExclusionTypes ?? Enumerable.Empty<Type>());
            
        
            foreach(var func in FunctionResolver.GetFunctions(assembly, ExcludedParameterTypes))
            {

                if(!doc.Paths.TryGetValue(func.Route, out var _))
                {
                    var responses = new OpenApiResponses();

                    foreach(var r in func.Returns)
                    {
                        if(r.BodyType != null) {
                            var primitive = r.BodyType.MapPrimitive();

                            if(primitive == null) {
                                doc.Components.Schemas.Merge(r.BodyType?.GetSchemas());

                                responses.Add(r.StatusCode.ToString(), new OpenApiResponse { 
                                    Description = r.Description,
                                    Content = new Dictionary<string, OpenApiMediaType> {
                                        ["application/json"] = new OpenApiMediaType { 
                                            Schema = new OpenApiSchema {
                                                Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = r.BodyType.Name }
                                            } 
                                        }
                                    }
                                });
                            } else {
                                responses.Add(r.StatusCode.ToString(), new OpenApiResponse { 
                                    Description = r.Description,
                                    Content = new Dictionary<string, OpenApiMediaType> {
                                        ["application/json"] = new OpenApiMediaType { 
                                            Schema = new OpenApiSchema {
                                                Type = r.BodyType.MapPrimitive()?.Type
                                            } 
                                        }
                                    }
                                });
                            }
                        } else {
                            responses.Add(r.StatusCode.ToString(), new OpenApiResponse {
                                Description = r.Description
                            });
                        }
                    }

                    doc.Paths[func.Route] = new OpenApiPathItem {
                        Description = func.Description,
                        Parameters = func.Parameters.Select(p =>
                            new OpenApiParameter {
                                In = ParameterLocation.Path,
                                Name = p.Name,
                                Schema = p.ParameterType.MapPrimitive(),
                                Required = true                                
                            }
                        ).ToList(),
                        Operations = func.Methods.ToDictionary(m => m, m => new OpenApiOperation {
                            Responses = responses
                        })
                    };
                }   
            }
                        
            return doc;
        }
    }

}
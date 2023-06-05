using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Namotion.Reflection;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Converters;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Unchase.OpenAPI.ConnectedService.CodeGeneration
{

    /// <summary>
    /// Decorates the Open API specification document with known inherited class types using "OneOf" and "AnyOf".
    /// Source from: https://gist.github.com/icnocop/10de946939e5046190219cc8817356c4
    /// Mentioned here: https://github.com/RicoSuter/NJsonSchema/pull/839
    /// </summary>
    /// <seealso cref="NSwag.Generation.Processors.IOperationProcessor" />
    public class OneOfOperationProcessor : IOperationProcessor
    {
        private const string mediaTypeName = "application/json"; // MediaTypeNames.Application.Json; only available with .Net Core 2.1+

        /// <inheritdoc/>
        public bool Process(OperationProcessorContext context)
        {
            this.SetRequests(context);

            this.SetResponses(context);

            return true;
        }

        private static JsonSchema GetSchemaForType(
            OperationProcessorContext context,
            Type type)
        {
            if (!context.SchemaResolver.HasSchema(type, false))
            {
                return null;
            }

            JsonSchema schema = context.SchemaResolver.GetSchema(type, false);

            return new JsonSchema
            {
                Reference = schema,
            };
        }

        private void SetRequests(OperationProcessorContext context)
        {
            if (context.OperationDescription.Operation.RequestBody == null)
            {
                return;
            }

            if (!context.OperationDescription.Operation.RequestBody.Content.ContainsKey(mediaTypeName))
            {
                return;
            }

            var mediaType = context.OperationDescription.Operation.RequestBody.Content[mediaTypeName];
            var apiParameter = context.OperationDescription.Operation.Parameters.Single(x => x.Kind == OpenApiParameterKind.Body);

            var parameter = context.Parameters.SingleOrDefault(x => x.Value.Name == apiParameter.Name);
            if (parameter.Equals(default(KeyValuePair<ParameterInfo, OpenApiParameter>)))
            {
                return;
            }

            var parameterType = parameter.Key.ParameterType;

            var newSchema = this.GenerateSchemaWithInheritanceForType(context, parameterType, false);
            if (newSchema != null)
            {
                mediaType.Schema = newSchema;
            }
        }

        private void SetResponses(OperationProcessorContext context)
        {
            var attributes = context.MethodInfo.GetCustomAttributes<ProducesResponseTypeAttribute>(true);
            foreach (var apiResponse in context.OperationDescription.Operation.Responses)
            {
                if (!apiResponse.Value.Content.ContainsKey(mediaTypeName))
                {
                    continue;
                }

                var mediaType = apiResponse.Value.Content[mediaTypeName];

                if (!int.TryParse(apiResponse.Key, out int responseStatusCode))
                {
                    continue;
                }

                var attribute = attributes.SingleOrDefault(x => x.StatusCode == responseStatusCode);
                if (attribute == null)
                {
                    continue;
                }

                var responseType = attribute.Type;

                var newSchema = this.GenerateSchemaWithInheritanceForType(context, responseType, false);
                if (newSchema != null)
                {
                    mediaType.Schema = newSchema;
                }
            }
        }

        private JsonSchema GenerateSchemaWithInheritanceForType(
            OperationProcessorContext context,
            Type type,
            bool includeBaseReference = true)
        {
            if (type.IsGenericType)
            {
                Type[] genericArguments = type.GetGenericArguments();

                if (genericArguments.Length == 1)
                {
                    Type genericArgumentType = genericArguments[0];

                    Type enumerableType = typeof(IEnumerable<>).MakeGenericType(genericArguments);
                    if (enumerableType.IsAssignableFrom(type))
                    {
                        return new JsonSchema
                        {
                            Type = JsonObjectType.Array,
                            Item = this.GenerateSchemaWithInheritanceForType(
                                context,
                                genericArgumentType,
                                false),
                            IsAbstract = true,
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            var knownTypeAttributes = type.GetCustomAttributes<KnownTypeAttribute>(true);
            if (!knownTypeAttributes.Any())
            {
                return null;
            }

            JsonSchema baseTypeSchema;

            if (includeBaseReference)
            {
                baseTypeSchema = GetSchemaForType(context, type);
                if (baseTypeSchema == null)
                {
                    return null;
                }

                if (baseTypeSchema.OneOf.Any())
                {
                    return baseTypeSchema;
                }

                baseTypeSchema.Title = null;
                baseTypeSchema.Type = JsonObjectType.None;
            }
            else
            {
                baseTypeSchema = new JsonSchema();
            }

            var discriminatorConverter = this.TryGetInheritanceDiscriminatorConverter(type);
            var discriminatorName = this.TryGetInheritanceDiscriminatorName(discriminatorConverter);

            JsonSchema typeSchema = GetSchemaForType(context, type);
            if (typeSchema == null)
            {
                return null;
            }

            this.GenerateInheritanceDiscriminator(
                baseTypeSchema,
                discriminatorConverter,
                discriminatorName,
                type,
                typeSchema);

            baseTypeSchema.OneOf.Add(new JsonSchema
            {
                Reference = typeSchema,
            });

            foreach (var attribute in knownTypeAttributes)
            {
                var knownTypeSchema = GetSchemaForType(context, attribute.Type);
                if (knownTypeSchema == null)
                {
                    continue;
                }

                baseTypeSchema.OneOf.Add(new JsonSchema
                {
                    Reference = knownTypeSchema,
                });

                // apply to properties
                foreach (PropertyInfo propertyInfo in attribute.Type.GetProperties())
                {
                    var propertyType = propertyInfo.PropertyType;
                    if (!context.SchemaResolver.HasSchema(propertyType, false))
                    {
                        continue;
                    }

                    if (context.Document.Components.Schemas.ContainsKey(propertyType.Name))
                    {
                        JsonSchema propertyTypeSchema = context.SchemaResolver.GetSchema(propertyType, false);
                        if ((propertyTypeSchema == null)
                            || propertyTypeSchema.AnyOf.Any())
                        {
                            continue;
                        }

                        var newSchema = this.GenerateSchemaWithInheritanceForType(context, propertyType, false);
                        if (newSchema != null)
                        {
                            var propertyTypeName = propertyType.Name;
                            foreach (var schema in newSchema.OneOf.Skip(1))
                            {
                                context.Document.Components.Schemas[propertyTypeName].AnyOf.Add(
                                    new JsonSchema
                                    {
                                        Reference = schema,
                                    });
                            }
                        }
                    }
                }
            }

            return baseTypeSchema;
        }

        private void GenerateInheritanceDiscriminator(
            JsonSchema baseSchema,
            object discriminatorConverter,
            string discriminatorName,
            Type knownType,
            JsonSchema knownTypeSchema)
        {
            this.AddDiscriminatorObject(baseSchema, discriminatorConverter, discriminatorName);
            this.AddDiscriminatorObject(knownTypeSchema, discriminatorConverter, discriminatorName);

            var baseDiscriminator = baseSchema.ResponsibleDiscriminatorObject ?? baseSchema.ActualTypeSchema.ResponsibleDiscriminatorObject;
            baseDiscriminator?.AddMapping(knownType, knownTypeSchema);
        }

        private void AddDiscriminatorObject(
            JsonSchema schema,
            object discriminatorConverter,
            string discriminatorName)
        {
            if (schema.DiscriminatorObject != null)
            {
                return;
            }

            var discriminator = new OpenApiDiscriminator
            {
                JsonInheritanceConverter = discriminatorConverter,
                PropertyName = discriminatorName,
            };

            schema.DiscriminatorObject = discriminator;

            if (schema.Properties.ContainsKey(discriminatorName))
            {
                return;
            }

            schema.Properties[discriminatorName] = new JsonSchemaProperty
            {
                Type = JsonObjectType.String,
                IsRequired = true,
                MinLength = 1,
            };
        }

        private object TryGetInheritanceDiscriminatorConverter(Type type)
        {
            var typeAttributes = type.GetTypeInfo().GetCustomAttributes(false).OfType<Attribute>();

            dynamic jsonConverterAttribute = typeAttributes.FirstAssignableToTypeNameOrDefault(nameof(JsonConverterAttribute), TypeNameStyle.Name);
            if (jsonConverterAttribute != null)
            {
                var converterType = (Type)jsonConverterAttribute.ConverterType;
                if (converterType != null && (

                    // Newtonsoft's converter
                    converterType.IsAssignableToTypeName(nameof(JsonInheritanceConverter), TypeNameStyle.Name)

                    // System.Text.Json's converter
                    || converterType.IsAssignableToTypeName(nameof(JsonInheritanceConverter) + "`1", TypeNameStyle.Name)))
                {
                    return ObjectExtensions.HasProperty(jsonConverterAttribute, "ConverterParameters") &&
                           jsonConverterAttribute.ConverterParameters != null &&
                           jsonConverterAttribute.ConverterParameters.Length > 0 ?
                        Activator.CreateInstance(jsonConverterAttribute.ConverterType, jsonConverterAttribute.ConverterParameters) :
                        Activator.CreateInstance(jsonConverterAttribute.ConverterType);
                }
            }

            return null;
        }

        private string TryGetInheritanceDiscriminatorName(object jsonInheritanceConverter)
        {
            return ObjectExtensions.TryGetPropertyValue(
                jsonInheritanceConverter,
                nameof(JsonInheritanceConverter.DiscriminatorName),
                JsonInheritanceConverter.DefaultDiscriminatorName);
        }
    }
}

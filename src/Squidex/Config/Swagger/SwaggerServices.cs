﻿// ==========================================================================
//  SwaggerServices.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;
using NSwag.AspNetCore;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors.Security;
using Squidex.Controllers.ContentApi.Generator;
using Squidex.Infrastructure;
using Squidex.Pipeline.Swagger;

namespace Squidex.Config.Swagger
{
    public static class SwaggerServices
    {
        public static void AddMySwaggerSettings(this IServiceCollection services)
        {
            services.AddSingleton(typeof(SwaggerOwinSettings), s =>
            {
                var options = s.GetService<IOptions<MyUrlsOptions>>().Value;

                var settings =
                    new SwaggerOwinSettings { Title = "Squidex API Specification", IsAspNetCore = false }
                        .ConfigurePaths()
                        .ConfigureSchemaSettings()
                        .ConfigureIdentity(options);

                return settings;
            });

            services.AddTransient<SchemasSwaggerGenerator>();
        }

        private static SwaggerOwinSettings ConfigureIdentity(this SwaggerOwinSettings settings, MyUrlsOptions urlOptions)
        {
            settings.DocumentProcessors.Add(
                new SecurityDefinitionAppender("OAuth2", SwaggerHelper.CreateOAuthSchema(urlOptions)));

            settings.OperationProcessors.Add(new OperationSecurityScopeProcessor("roles"));

            return settings;
        }

        private static SwaggerOwinSettings ConfigurePaths(this SwaggerOwinSettings settings)
        {
            settings.SwaggerRoute = $"{Constants.ApiPrefix}/swagger/v1/swagger.json";

            settings.PostProcess = document =>
            {
                document.BasePath = Constants.ApiPrefix;
            };

            settings.MiddlewareBasePath = Constants.ApiPrefix;

            return settings;
        }

        private static SwaggerOwinSettings ConfigureSchemaSettings(this SwaggerOwinSettings settings)
        {
            settings.DefaultEnumHandling = EnumHandling.String;
            settings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;

            settings.TypeMappers = new List<ITypeMapper>
            {
                new PrimitiveTypeMapper(typeof(Language), s => s.Type = JsonObjectType.String),
                new PrimitiveTypeMapper(typeof(RefToken), s => s.Type = JsonObjectType.String)
            };

            settings.DocumentProcessors.Add(new XmlTagProcessor());

            settings.OperationProcessors.Add(new XmlTagProcessor());
            settings.OperationProcessors.Add(new XmlResponseTypesProcessor());

            return settings;
        }
    }
}
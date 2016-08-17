// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Internal
{
    public class MiddlewareFilterTest
    {
        [Fact]
        public void MiddlewareFilter_ThrowsException_OnNoRequestDelegate()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MiddlewareFilter(requestDelegate: null));
        }

        [Fact]
        public async Task MiddlewareFilter_SetsMiddlewareFilterFeature_OnExecution()
        {
            // Arrange
            RequestDelegate requestDelegate = (context) => Task.FromResult(true);
            var middlwareFilter = new MiddlewareFilter(requestDelegate);
            var httpContext = new DefaultHttpContext();
            var resourceExecutingContext = GetResourceExecutingContext(httpContext);
            var resourceExecutionDelegate = GetResourceExecutionDelegate(httpContext);

            // Act
            await middlwareFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceExecutionDelegate);

            // Assert
            var feature = resourceExecutingContext.HttpContext.Features.Get<IMiddlewareFilterFeature>();
            Assert.NotNull(feature);
            Assert.Same(resourceExecutingContext, feature.ResourceExecutingContext);
            Assert.Same(resourceExecutionDelegate, feature.ResourceExecutionDelegate);
        }

        [Fact]
        public async Task MiddlewareFilter_ExecutesMiddleware_OnExecution()
        {
            // Arrange
            var expectedResponseData = "Hello!";
            RequestDelegate requestDelegate = (context) =>
            {
                context.Response.StatusCode = 200;
                return context.Response.WriteAsync(expectedResponseData);
            };
            var middlwareFilter = new MiddlewareFilter(requestDelegate);
            var responseStream = new MemoryStream();
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = responseStream;
            var resourceExecutingContext = GetResourceExecutingContext(httpContext);
            var resourceExecutionDelegate = GetResourceExecutionDelegate(httpContext);

            // Act
            await middlwareFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceExecutionDelegate);

            // Assert
            Assert.Equal(200, httpContext.Response.StatusCode);
            responseStream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(responseStream);
            Assert.Equal(expectedResponseData, streamReader.ReadToEnd());
        }

        private ResourceExecutingContext GetResourceExecutingContext(HttpContext httpContext)
        {
            return new ResourceExecutingContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), new ModelStateDictionary()),
                new List<IFilterMetadata>(),
                new List<IValueProviderFactory>());
        }

        private ResourceExecutionDelegate GetResourceExecutionDelegate(HttpContext httpContext)
        {
            return new ResourceExecutionDelegate(
                () => Task.FromResult(new ResourceExecutedContext(new ActionContext(), new List<IFilterMetadata>())));
        }
    }
}

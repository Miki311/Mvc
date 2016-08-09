using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.Internal
{
    /// <summary>
    /// Builds a middleware pipeline after receiving the pipeline from a pipeline provider
    /// </summary>
    public class MiddlewarePipelineBuilderService
    {
        private readonly ConcurrentDictionary<Type, Lazy<RequestDelegate>> _pipelinesCache
            = new ConcurrentDictionary<Type, Lazy<RequestDelegate>>();

        public IApplicationBuilder ApplicationBuilder { get; set; }

        public RequestDelegate GetPipeline(Type type)
        {
            // Build the pipeline only once. This is similar to how middlewares are used where they are constructed
            // only once.

            var requestDelegate = _pipelinesCache.GetOrAdd(
                type,
                key => new Lazy<RequestDelegate>(() => BuildPipeline(key)));

            return requestDelegate.Value;
        }

        private RequestDelegate BuildPipeline(Type type)
        {
            var pipelineProvider = (IMiddlewarePipelineProvider)Activator.CreateInstance(type);

            var nestedAppBuilder = ApplicationBuilder.New();

            // Get the user provided pipeline
            pipelineProvider.Configure(nestedAppBuilder);

            // Attach a middleware in the end so that it continues the execution of rest of the MVC filter pipeline
            nestedAppBuilder.Run((httpContext) =>
            {
                var filterContext = httpContext.Items[typeof(ResourceFilterContext)] as ResourceFilterContext;
                var resourceExecutionDelegate = filterContext.ResourceExecutionDelegate;

                return resourceExecutionDelegate();
            });

            return nestedAppBuilder.Build();
        }
    }
}

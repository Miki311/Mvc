using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.Core.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MiddlewareFilterAttribute : Attribute, IFilterFactory, IOrderedFilter
    {
        public MiddlewareFilterAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(IMiddlewarePipelineProvider).GetTypeInfo().IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    $"Supplied type {type} must implement {typeof(IMiddlewarePipelineProvider).ToString()}");
            }

            ImplementationType = type;
        }

        public Type ImplementationType { get; }

        public int Order { get; set; }

        public bool IsReusable { get; set; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var middlewarePipelineService = serviceProvider.GetRequiredService<MiddlewarePipelineBuilderService>();
            var pipeline = middlewarePipelineService.GetPipeline(ImplementationType);

            return new MiddlewareFilter(pipeline);
        }
    }
}

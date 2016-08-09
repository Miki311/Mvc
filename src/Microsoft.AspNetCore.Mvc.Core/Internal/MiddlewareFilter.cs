using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.Internal
{
    public class MiddlewareFilter : IAsyncResourceFilter, IOrderedFilter
    {
        private readonly RequestDelegate _requestDelegate;

        public MiddlewareFilter(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public int Order
        {
            get
            {
                return int.MinValue + 100;
            }
        }

        public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var httpContext = context.HttpContext;

            httpContext.Items.Add(
                typeof(ResourceFilterContext),
                new ResourceFilterContext()
                {
                    ResourceExecutionDelegate = next,
                    ResourceExecutingContext = context
                });

            return _requestDelegate(httpContext);
        }
    }
}

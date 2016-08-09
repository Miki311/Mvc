using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.Internal
{
    public class ResourceFilterContext
    {
        public ResourceExecutingContext ResourceExecutingContext { get; set; }

        public ResourceExecutionDelegate ResourceExecutionDelegate { get; set; }
    }
}

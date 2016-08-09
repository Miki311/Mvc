using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Mvc
{
    public interface IMiddlewarePipelineProvider
    {
        void Configure(IApplicationBuilder applicationBuilder);
    }
}

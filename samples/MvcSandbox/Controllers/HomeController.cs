// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Filters;

namespace MvcSandbox.Controllers
{
    public class HomeController : Controller
    {
        [MiddlewareFilter(typeof(Pipeline1))]
        public IActionResult Index()
        {
            return Content("Home.Index");
        }

        [MiddlewareFilter(typeof(Pipeline2))]
        public IActionResult Contact()
        {
            return Content("Home.Contact");
        }
    }

    [MiddlewareFilter(typeof(Pipeline2))]
    public class FooController : Controller
    {
        public IActionResult Index()
        {
            return Content("Foo.Index");
        }
    }

    public class Pipeline1 : IMiddlewarePipelineProvider
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Use(async (httpContext, next) =>
            {
                Console.WriteLine("Action1_MiddlewarePipeline: Use1-Request");
                await next();
                Console.WriteLine("Action1_MiddlewarePipeline: Use1-Response");
            });

            applicationBuilder.Use(async (httpContext, next) =>
            {
                Console.WriteLine("Action1_MiddlewarePipeline: Use2-Request");
                await next();
                Console.WriteLine("Action1_MiddlewarePipeline: Use2-Response");
            });
        }
    }

    public class Pipeline2 : IMiddlewarePipelineProvider
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Use(async (httpContext, next) =>
            {
                Console.WriteLine("Action1_MiddlewarePipeline: Use1-Request");
                await next();
                Console.WriteLine("Action1_MiddlewarePipeline: Use1-Response");
            });
        }
    }
}

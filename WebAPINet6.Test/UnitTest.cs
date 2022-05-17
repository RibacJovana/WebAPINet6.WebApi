using WebAPINet6.WebApi.Controllers;
using Xunit;
using WebAPINet6.WebApi;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPINet6.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using WebAPINet6.WebApi.Middleware;
using Microsoft.Extensions.Logging;
using Moq;

namespace WebAPINet6.Test
{
    public class UnitTest
    {

        [Fact]
        public async Task TestLoggingMiddleware()
        {
            var logger = new Mock<ILogger<LoggingMiddleware>>();
            var context = new DefaultHttpContext();
            context.Request.RouteValues["ids"] = "tts-78738373";
            var wasExecuted = false;

            RequestDelegate next = (HttpContext ctx) => 
            { 
                wasExecuted = true;
                return Task.CompletedTask;
            };

            var middleware = new LoggingMiddleware((ILogger<LoggingMiddleware>)logger);
            await middleware.InvokeAsync(context, next);

            Assert.True(wasExecuted);
        }
    }
}
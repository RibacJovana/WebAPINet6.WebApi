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
        Mock<ILogger<LoggingMiddleware>> logger = new Mock<ILogger<LoggingMiddleware>>();

        [Fact]
        public async Task TestLoggingMiddleware_Valid_ID()
        {
            var context = new DefaultHttpContext();
            context.Request.RouteValues["ids"] = "tts-78738373 tts-78738373";

            var wasExecuted = false;

            RequestDelegate next = (HttpContext ctx) => 
            { 
                wasExecuted = true;
                return Task.CompletedTask;
            };

            var middleware = new LoggingMiddleware(logger.Object);
            await middleware.InvokeAsync(context, next);

            Assert.True(wasExecuted);
        }

        [Fact]
        public async Task TestLoggingMiddleware_Invalid_ID()
        {
            var context = new DefaultHttpContext();
            context.Request.RouteValues["ids"] = "tts-7873safsa8373";

            var wasExecuted = false;

            RequestDelegate next = (HttpContext ctx) =>
            {
                wasExecuted = true;
                return Task.CompletedTask;
            };

            var middleware = new LoggingMiddleware(logger.Object);
            await middleware.InvokeAsync(context, next);

            Assert.False(wasExecuted);
            Assert.Equal("text/plain", context.Response.ContentType);
            Assert.Equal(404, context.Response.StatusCode);
        }
    }
}
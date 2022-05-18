using Microsoft.AspNetCore.Http.Extensions;
using System.Text.RegularExpressions;

namespace WebAPINet6.WebApi.Middleware
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
        { 
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Console.WriteLine(context.Request.GetDisplayUrl());

            try
            {
                string regex = @"^\s*(tts-[0-9]+\s?)+\s*$";
                var ids = context.Request.RouteValues["ids"].ToString();

                if (Regex.Match(ids, regex).Success)
                {
                    _logger.LogInformation("ID has good format!");
                    await next.Invoke(context);
                }
                else
                {
                    _logger.LogWarning("ID has bad format!");

                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Error!\n Id has bad format!\n It should be: tts-br / tts-br tts-br tts-br.. ");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while executing request.");
            }
        }
    }
}

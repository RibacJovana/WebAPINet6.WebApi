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
                var value = context.Request.RouteValues["ids"];

                if (Regex.Match(value.ToString(), regex).Success)
                {
                    _logger.LogInformation("ID: {id} je dobrog formata!", value);
                    await next.Invoke(context);
                }
                else
                {
                    _logger.LogWarning("ID: {id} je loseg formata!");

                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Greska! Id ima los format, mora da bude u sledecem obliku: tts-br / tts-br tts-br tts-br.. ");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while executing request.");
            }
        }
    }
}

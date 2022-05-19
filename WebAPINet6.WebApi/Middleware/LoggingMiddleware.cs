using Microsoft.AspNetCore.Http.Extensions;
using System.Net;
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
                var ids = context.Request.RouteValues["ids"];

                if (Regex.Match(ids!.ToString()!, regex).Success)
                {
                    _logger.LogInformation("ID has good format!");
                    await next.Invoke(context);
                }
                else
                {
                    _logger.LogWarning("ID has bad format!");
                    throw new BadHttpRequestException("Error!\n Id has bad format!\n It should be: tts-br / tts-br tts-br tts-br.. ");
                }
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError(ex, "Problem with request.");
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Server is temporarily unavailable.");
                await HandleExceptionAsync(context, ex, HttpStatusCode.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generic error has occurred on the server.");
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode httpStatusCode, string content_type = "text/plain")
        {
            context.Response.ContentType = content_type;
            context.Response.StatusCode = (int)httpStatusCode;
            return context.Response.WriteAsync(exception.Message);
        }
    }
}

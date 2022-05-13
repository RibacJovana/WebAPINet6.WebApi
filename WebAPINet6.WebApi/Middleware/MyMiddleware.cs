using System.Text.RegularExpressions;

namespace WebAPINet6.WebApi.Middleware
{
    public class MyMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var regex = @"^\s*(tts-[0-9]+\s?)+\s*$";
            string? value = context.Request.RouteValues["ids"].ToString();
          
            if (Regex.Match(value, regex).Success)
            {
                await next.Invoke(context);
            }
            else
            { 
                context.Response.StatusCode = 404;
            }
        }
    }
}

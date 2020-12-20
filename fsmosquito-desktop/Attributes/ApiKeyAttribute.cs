namespace FsMosquito
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using System;
    using System.Threading.Tasks;

    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Unauthorized - an API Key must be provided"
                };
                return;
            }

            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<FsMosquitoOptions>>();

            var apiKey = "1234";

            if (options != null && options.Value != null && !string.IsNullOrWhiteSpace(options.Value.ApiKey))
            {
                apiKey = options.Value.ApiKey;
            }

            if (!apiKey.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Unauthorized - API Key is not valid"
                };
                return;
            }

            await next();
        }
    }
}

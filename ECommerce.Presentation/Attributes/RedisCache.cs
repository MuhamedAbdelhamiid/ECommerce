using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Presentation.Attributes
{
    public class RedisCache : ActionFilterAttribute
    {
        private readonly double _durationInMinutes;

        public RedisCache(double durationInMinutes)
        {
            _durationInMinutes = durationInMinutes;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            var cacheService =
                context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

            var cacheKey = CeateCacheKey(context.HttpContext.Request);

            var value = await cacheService.GetCacheAsync(cacheKey);

            if (value is not null)
            {
                context.Result = new ContentResult()
                {
                    Content = value,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK,
                };

                return;
            }
            else
            {
                var executedContext = await next.Invoke();
                if (executedContext.Result is OkObjectResult okResult)
                {
                    await cacheService.SetCacheAsync(
                        cacheKey,
                        okResult.Value!,
                        TimeSpan.FromMinutes(_durationInMinutes)
                    );
                }
            }
        }

        private string CeateCacheKey(HttpRequest request)
        {
            var cacheKey = new StringBuilder();

            cacheKey.Append(request.Path);

            foreach (var query in request.Query.OrderBy(q => q.Key))
                cacheKey.Append($"|{query.Key}-{query.Value}");

            return cacheKey.ToString();
        }
    }
}

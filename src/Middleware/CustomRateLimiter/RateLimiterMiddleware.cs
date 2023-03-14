using CustomRateLimiter.Attributes;
using CustomRateLimiter.Extensions;
using CustomRateLimiter.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace CustomRateLimiter
{
	internal class RateLimiterMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IDistributedCache _cache;
		private readonly IOptions<RateLimiterOptions> _options;

		public RateLimiterMiddleware(RequestDelegate next, IDistributedCache cache, IServiceProvider serviceProvider)
		{
			next.ThrowIfNull();
			cache.ThrowIfNull();
			serviceProvider.ThrowIfNull();

			_next = next;
			_cache = cache;
			serviceProvider.ThrowIfNull();
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
			if (endpoint?.Metadata.GetMetadata<DisableRateLimiterAttribute>() is not null)
			{
				await _next.Invoke(context);
			}

			_options.Value switch
			{
				SimpleRateLimiterOptions simpleRateLimiterOptions => await InvokeSimpleRateLimiterAsync(context, simpleRateLimiterOptions),
				TokenBucketRateLimiterOptions tokenBucketRateLimiterOptions => tokenBucketRateLimiterOptions,
				WindowRateLimiterOptions windowRateLimiterOptions => windowRateLimiterOptions,
				SlidingRateLimiterOptions slidingRateLimiterOptions => slidingRateLimiterOptions,
			};

			await _next.Invoke(context);
		}

		private async Task InvokeSimpleRateLimiterAsync(HttpContext context, SimpleRateLimiterOptions options)
		{
			var endpointCallsString = await _cache.GetStringAsync(context.Request.Path);
			if (!string.IsNullOrEmpty(endpointCallsString) && int.TryParse(endpointCallsString, out var endpointCalls))
			{
				if (endpointCalls > 0 && endpointCalls < options.EndpointCalls)
				{
					await _cache.SetStringAsync(context.Request.Path, (++endpointCalls).ToString());
				}
				else
				{
					await _cache.RemoveAsync(context.Request.Path);
				}
			}
			else
			{
				await _cache.SetStringAsync(context.Request.Path, "1", new DistributedCacheEntryOptions()
				{
					AbsoluteExpiration = new DateTimeOffset().Add(options.Period)
				});
			}
		}

		private async Task InvokeTokenBucketRateLimiterAsync(HttpContext context, TokenBucketRateLimiterOptions options)
		{
			// Use new dotnet periodic timer
		}
	}
}

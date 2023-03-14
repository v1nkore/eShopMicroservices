using Microsoft.AspNetCore.Http;

namespace CustomRateLimiter.Options
{
	internal class RateLimiterOptions
	{
		public int EndpointCalls { get; init; }
		public int RejectionStatusCode { get; init; } = StatusCodes.Status429TooManyRequests;
	}
}

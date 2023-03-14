namespace CustomRateLimiter.Options
{
	internal class SimpleRateLimiterOptions : RateLimiterOptions
	{
		public TimeSpan Period { get; init; }
	}
}

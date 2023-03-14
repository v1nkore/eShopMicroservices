namespace CustomRateLimiter.Options;

internal class TokenBucketRateLimiterOptions : RateLimiterOptions
{
	public int AdditionTokens { get; set; }
	public TimeSpan Period { get; set; }
}
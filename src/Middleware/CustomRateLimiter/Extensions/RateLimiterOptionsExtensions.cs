using CustomRateLimiter.Options;

namespace CustomRateLimiter.Extensions
{
	internal static class RateLimiterOptionsExtensions
	{
		public static void AddSimpleRateLimiter(
			this RateLimiterOptions options,
			Action<SimpleRateLimiterOptions> configure)
		{
			configure.ThrowIfNull();

			var simpleRateLimiterOptions = new SimpleRateLimiterOptions();
			configure(simpleRateLimiterOptions);
		}

		public static void AddTokenBucketRateLimiter(
			this RateLimiterOptions options,
			Action<TokenBucketRateLimiterOptions> configure)
		{
			configure.ThrowIfNull();

			var tokenBucketRateLimiterOptions = new TokenBucketRateLimiterOptions();
			configure(tokenBucketRateLimiterOptions);
		}

		public static void AddWindowTokenLimiter()
		{

		}

		public static void AddSlidingWindowTokenLimiter()
		{

		}
	}
}

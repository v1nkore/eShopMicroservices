namespace CustomRateLimiter.Extensions
{
	internal static class ThrowHelper
	{
		public static void ThrowIfNull<TValue>(this TValue? value, string? message = null)
		{
			if (value is null)
			{
				throw new ArgumentNullException(nameof(value), message);
			}
		}
	}
}

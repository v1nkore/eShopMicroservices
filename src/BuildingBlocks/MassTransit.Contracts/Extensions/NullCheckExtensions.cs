namespace MassTransit.Contracts.Extensions
{
	public static class NullCheckExtensions
	{
		public static void ThrowIfNull<T>(T? value) where T : class
		{
			if (value is null)
			{
				throw new ArgumentNullException(nameof(value));
			}
		}

		public static void ThrowIfNull(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				throw new ArgumentNullException(nameof(str));
			}
		}
	}
}

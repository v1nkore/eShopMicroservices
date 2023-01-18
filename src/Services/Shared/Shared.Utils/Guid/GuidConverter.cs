namespace Shared.Utils.Guid
{
	public static class GuidConverter
	{
		public static string Encode(System.Guid guid)
		{
			return Convert.ToBase64String(guid.ToByteArray())
				.Replace('/', '_')
				.Replace('+', '-')
				.Substring(0, 22);
		}

		public static System.Guid Decode(string? encodedGuid)
		{
			if (encodedGuid is null)
			{
				return ServerSideGuidGenerator.CreateGuid();
			}

			encodedGuid = encodedGuid.Replace('_', '/').Replace('-', '+');
			var buffer = Convert.FromBase64String(encodedGuid + "==");

			return new System.Guid(buffer);
		}
	}
}

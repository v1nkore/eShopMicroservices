using System.Runtime.InteropServices;

namespace Shared.Utils.Guid
{
	public static class ServerSideGuidGenerator
	{
		public static System.Guid CreateGuid()
		{
			int result = NativeMethods.UuidCreateSequential(out var guid);
			if (result == 0)
			{
				var bytes = guid.ToByteArray();
				var indexes = new[] { 3, 2, 1, 0, 5, 4, 7, 6, 8, 9, 10, 11, 12, 13, 14, 15 };
				return new System.Guid(indexes.Select(i => bytes[i]).ToArray());
			}
			else
				throw new Exception("Error generating sequential GUID");
		}

		public static class NativeMethods
		{
			[DllImport("rpcrt4.dll", SetLastError = true)]
			public static extern int UuidCreateSequential(out System.Guid guid);
		}
	}
}

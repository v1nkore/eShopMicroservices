using Grpc.Core;

namespace Discount.GRPC.Services
{
	public class TestService : TestProto.TestProtoBase
	{
		public override async Task<TestResponse> TestGetString(Empty request, ServerCallContext context)
		{
			return await Task.FromResult(new TestResponse() { ResponseMessage = "Test response message" });
		}
	}
}
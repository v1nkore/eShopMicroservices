namespace Shared.Responses.ServiceResponses
{
	public class ServiceResponse<TValue, TError>
	{
		public TValue? Value { get; set; }
		public TError? Error { get; set; }
		public ServiceResponseStatus Status { get; set; }
	}
}
using MediatR;

namespace Ordering.Application.Behaviours
{
	public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	{
		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			try
			{
				return await next();
			}
			catch (Exception e)
			{
				var type = typeof(TRequest);
				if (type.IsClass)
				{
					// TODO: Log error with type.Name

				}

				// TODO: Log error

				throw;
			}
		}
	}
}
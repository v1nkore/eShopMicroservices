using FluentValidation.Results;

namespace Ordering.Application.Exceptions
{
	public class ValidationException : ApplicationException
	{
		public IDictionary<string, string[]> Errors { get; set; }

		public ValidationException() : base("One or more validation exceptions have occurred.")
		{
			Errors = new Dictionary<string, string[]>();
		}

		public ValidationException(IEnumerable<ValidationFailure> failures)
		{
			Errors = failures
				.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
				.ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
		}
	}
}
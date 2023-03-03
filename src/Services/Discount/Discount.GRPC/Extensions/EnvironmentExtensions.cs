namespace Discount.GRPC.Extensions;

public static class EnvironmentExtensions
{
	public const string IntegrationTestingEnvironmentName = "Integration testing";

	public static bool IsIntegrationTesting(this IWebHostEnvironment environment)
	{
		return environment.EnvironmentName == IntegrationTestingEnvironmentName;
	}
}
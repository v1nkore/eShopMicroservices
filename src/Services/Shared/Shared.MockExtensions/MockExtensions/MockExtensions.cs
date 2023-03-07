using Moq;

namespace Shared.MockExtensions.MockExtensions
{
	public static class MockExtensions
	{
		public static void VerifyFindAsyncCall<T>(this Mock<IMongoCollection<T>> collectionMock)
		{
			collectionMock.Verify(v => v.FindAsync(
				It.IsAny<FilterDefinition<T>>(),
				It.IsAny<FindOptions<T>>(),
				It.IsAny<CancellationToken>()), Times.Once);
		}

		public static void VerifyMap<TSource, TDestination>(this Mock<IMapper> mapperMock, TSource source)
		{
			mapperMock.Verify(v => v.Map<TDestination>(source), Times.Once);
		}
	}
}

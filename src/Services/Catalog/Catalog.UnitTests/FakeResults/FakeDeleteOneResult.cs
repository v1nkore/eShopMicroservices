using MongoDB.Driver;

namespace Catalog.UnitTests.FakeResults
{
	internal sealed class FakeDeleteOneResult : DeleteResult
	{
		public override long DeletedCount { get; } = 1;
		public override bool IsAcknowledged { get; } = true;
	}
}

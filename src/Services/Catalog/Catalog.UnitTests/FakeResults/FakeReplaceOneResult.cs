using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.UnitTests.FakeResults
{
	internal sealed class FakeReplaceOneResult : ReplaceOneResult
	{
		public override bool IsAcknowledged { get; }
		public override bool IsModifiedCountAvailable { get; }
		public override long MatchedCount { get; }
		public override long ModifiedCount { get; }
		public override BsonValue UpsertedId { get; }

		public FakeReplaceOneResult(bool isAcknowledged, int modifiedCount)
		{
			IsAcknowledged = isAcknowledged;
			ModifiedCount = modifiedCount;
		}
	}
}

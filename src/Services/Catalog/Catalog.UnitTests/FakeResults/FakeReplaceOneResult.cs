using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.UnitTests.FakeResults
{
	internal sealed class FakeReplaceOneResult : ReplaceOneResult
	{
		public override bool IsAcknowledged { get; } = true;
		public override bool IsModifiedCountAvailable { get; }
		public override long MatchedCount { get; }
		public override long ModifiedCount { get; } = 1;
		public override BsonValue UpsertedId { get; }
	}
}

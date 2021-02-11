using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace iEnvironment.Domain.Models
{
    [BsonIgnoreExtraElements]
    public abstract class BsonObject
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; }
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}

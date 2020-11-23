using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iEnvironment.Domain.Models
{
    public abstract class BsonObject
    {
        [BsonId]
        public ObjectId Id { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}

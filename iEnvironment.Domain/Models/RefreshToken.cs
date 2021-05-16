using System;
using MongoDB.Bson.Serialization.Attributes;

namespace iEnvironment.Domain.Models
{
    public class RefreshToken : BsonObject
    {
        [BsonElement("value")]
        public string Value { get; set; }
        [BsonElement("expirationTime")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]

        public DateTime ExpirationTime { get; set; }
        [BsonElement("userId")]
        public string UserID { get; set; }

        public bool IsValid()
        {
            return DateTime.Now < ExpirationTime;
        }

        public RefreshToken(TimeSpan delay, string userId)
        {
            Value = Guid.NewGuid().ToString();
            ExpirationTime = DateTime.Now.Add(delay);
            UserID = userId;
        }
    }
}

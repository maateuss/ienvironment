using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace iEnvironment.Domain.Models
{
    public class DataVariation
    {
        [BsonElement("eqpId")]
        public string EqpId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("date")]
        public DateTime Date { get; set; }
        [BsonElement("value")]
        public object Value { get; set; }

        public DataVariation(string eqpId, string value)
        {
            Date = DateTime.Now;
            Value = value;
            EqpId = eqpId;
        }

    }
}

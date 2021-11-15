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
        [BsonElement("name")]
        public string EquipmentName { get; set; }
        [BsonElement("message")]
        public string CustomMessage { get; set; }
        [BsonElement("envName")]
        public string EnvironmentName { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("date")]
        public DateTime Date { get; set; }
        [BsonElement("value")]
        public object Value { get; set; }

        public DataVariation(string eqpId, string value, string eqpName, string envName, string customMessage)
        {
            Date = DateTime.Now;
            Value = value;
            EqpId = eqpId;
            EquipmentName = eqpName;
            CustomMessage = customMessage;
            EnvironmentName = envName;
        }
        
    }
}

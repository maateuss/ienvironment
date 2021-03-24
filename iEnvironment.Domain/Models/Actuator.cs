using System;
using MongoDB.Bson.Serialization.Attributes;

namespace iEnvironment.Domain.Models
{

    [BsonIgnoreExtraElements]
    public class Actuator : Equipment
    {
        [BsonElement("lastSignalReceivedTime")]
        public DateTime LastSignalReceivedTime { get; set; }

        public override bool ValidateNew()
        {
            return true;
        }

        public override Equipment ValidateUpdate()
        {
            return this;
        }
    }
}

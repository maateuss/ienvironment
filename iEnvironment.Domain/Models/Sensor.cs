using System;
using MongoDB.Bson.Serialization.Attributes;

namespace iEnvironment.Domain.Models
{

    [BsonIgnoreExtraElements]
    public class Sensor : Equipment
    {
        [BsonElement("measurementUnit")]
        public string MeasurementUnit { get; set; }
        [BsonElement("defaultTriggersActive")]
        public bool DefaultTriggersActive { get; set; }
        [BsonElement("limitUp")]
        public object LimitUp { get; set; }
        [BsonElement("limitDown")]
        public object LimitDown { get; set; }

    }
}

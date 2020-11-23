using System;
using iEnvironment.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace iEnvironment.Domain.Models
{
    public class Equipment : BsonObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public EquipmentDataType EntityType { get; set; }
        public string Topic { get; set; }
        public bool Connected { get; set; }
        public object CurrentValue { get; set; }
        [BsonIgnore]
        public Image Img { get; set; }
        public string ImageId { get; set; }
        public bool Enabled { get; set; }
        public bool SimulationMode { get; set; }
        public string Script { get; set; }
    }
}

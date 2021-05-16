using System;
using iEnvironment.Domain.Enums;
using iEnvironment.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace iEnvironment.Domain.Models
{
    public abstract class Equipment : BsonObject
    {
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("entityType")]
        public EquipmentDataType EntityType { get; set; }
        [BsonElement("topic")]
        public string Topic { get => GetTopic(); }

        private string GetTopic()
        {
            return $"sinal/{MicrocontrollerID}/{Id}/";
        }

        [BsonIgnore]
        public bool Alive { get => IsAlive(); }
        [BsonElement("connected")]
        public bool Connected { get; set; }
        [BsonElement("currentValue")]
        public object CurrentValue { get; set; }
        [BsonElement("enabled")]
        public bool Enabled { get; set; }
        [BsonElement("simulationMode")]
        public bool SimulationMode { get; set; }

        [BsonElement("keepAlive")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime KeepAlive { get; set; }

        [BsonElement("microcontrollerId")]
        public string MicrocontrollerID { get; set; }

        private int autoDisconnectSeconds = 300;
        [BsonElement("autoDisconnectSeconds")]
        public int AutoDisconnectSeconds
        {
            get => autoDisconnectSeconds;
            set
            {
                if(value < 0)
                {
                    throw new EquipmentMisconfiguratedException("AutoDisconnectSeconds must be a positive value!");
                }

                autoDisconnectSeconds = value;
            }
        }

        public abstract bool ValidateNew();
        public abstract Equipment ValidateUpdate();
        private bool IsAlive()
        {
            return DateTime.Now.Subtract(KeepAlive).TotalSeconds < AutoDisconnectSeconds;
        }

        public bool UpdateValue(object value, bool isConnecting)
        {
            if (!Enabled)
            {
                throw new EquipmentMisconfiguratedException("Equipment is disabled!");
            }

            if (!Alive && !isConnecting)
            {
                throw new EquipmentMisconfiguratedException("Equipment is not alive neither connecting!");
            }

            if (isConnecting)
            {
                KeepAlive = DateTime.Now;
            }

            UpdatedAt = DateTime.Now;
            CurrentValue = value;
            return true;
        }


    }
}

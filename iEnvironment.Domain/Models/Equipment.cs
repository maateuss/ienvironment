using System;
using iEnvironment.Domain.Enums;
using iEnvironment.Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace iEnvironment.Domain.Models
{
    public class Equipment : BsonObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public EquipmentDataType EntityType { get; set; }
        public string Topic { get; set; }
        [BsonIgnore]
        public bool Alive { get => IsAlive(); }
        public bool Connected { get; set; }
        public object CurrentValue { get; set; }
        [BsonIgnore]
        public Image Picture { get; set; }
        public string ImageId { get; set; }
        public bool Enabled { get; set; }
        public bool SimulationMode { get; set; }
        public string Script { get; set; }
        public DateTime KeepAlive { get; set; }

        private int autoDisconnectSeconds = 300;

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

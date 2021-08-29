using System;
namespace iEnvironment.Domain.Models
{
    public class Message
    {
        public string Payload { get; set; }
        public string Topic { get; set; }
        public bool IsValid()
        {
            return Payload.StartsWith("sinal/") || Payload.StartsWith("action/");
        }

        public bool IsSensor { get => Topic.StartsWith("sinal/"); }
        public bool IsActuator { get => Topic.StartsWith("action/"); }
        public bool TryGetMicroControllerId(out string microcontrollerId)
        {
            microcontrollerId = string.Empty;
            if (Topic?.Split('/').Length > 3)
            {
                microcontrollerId = Topic.Split('/')[1];
                return true;
            }
            else return false;
        }

        public bool TryGetEquipmentId(out string equipmentId)
        {
            equipmentId = string.Empty;
            if (Topic?.Split('/').Length > 3)
            {
                equipmentId = Topic.Split('/')[2];
                return true;
            }
            else return false;
        }


        public string GetEquipmentId()
        {
            if(TryGetEquipmentId(out string eqpid))
            {
                return eqpid;
            }
            return String.Empty;
        }

        public string GetMicrocontrollerId()
        {
            if (TryGetMicroControllerId(out string mcuid))
            {
                return mcuid;
            }
            return String.Empty;
        }

    }
}

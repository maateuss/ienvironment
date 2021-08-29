using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iEnvironment.Domain.Models
{

    [BsonIgnoreExtraElements]
    public class Environments : BsonObject
    {
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("equipments")]
        public List<string> Equipments { get; set; } = new List<string>();
        [BsonElement("events")]
        public List<string> Events { get; set; } = new List<string>();
        [BsonElement("enabled")]
        public bool Enabled { get; set; }
        [BsonElement("img")]
        public Image Img { get; set; }

        public bool AddEquipment(string id)
        {
            if (Equipments.Any(x => x == id))
            {
                return false;
            }
            Equipments.Add(id);
            return true;
        }

        public bool RemoveEquipment(string id)
        {
            if (!Equipments.Any(x => x == id))
            {
                return false;
            }
            Equipments.Remove(id);
            return true;
        }

        public bool AddEvent(string id)
        {
            if (Events.Any(x => x == id))
            {
                return false;
            }
            Events.Add(id);
            return true;
        }

        public bool RemoveEvent(string id)
        {
            if (!Events.Any(x => x == id))
            {
                return false;
            }
            Events.Remove(id);
            return true;
        }

        public bool ValidateNewEnvironment()
        {
            return true;
        }

        public Environments ValidateEnvironmentUpdate()
        {
            return this;
        }

    }
}

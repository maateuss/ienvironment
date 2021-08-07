using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace iEnvironment.Domain.Models
{
    [BsonIgnoreExtraElements]
    public class MicroController : BsonObject
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("connected")]
        public bool Connected { get; set; } = false;

        [BsonElement("img")]
        public Image Img { get; set; }

        [BsonElement("login")]
        public string Login { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("equipments")]
        public List<string> Equipments { get; set; } = new List<string>();

        [BsonElement("enabled")]
        public bool Enabled { get; set; }

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

        public bool ValidateNewMCU()
        {
            return true;
        }


        public MicroController ValidateMCUUpdate()
        {
            return this;
        }

    }
}

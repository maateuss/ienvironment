using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
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


        public static bool ValidateNewEnvironment(Environments env)
        {
            return true;
        }

        public static Environments ValidateEnvironmentUpdate(Environments env)
        {
            return env;
        }

    }
}

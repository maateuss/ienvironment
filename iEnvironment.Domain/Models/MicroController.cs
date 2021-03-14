﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
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
        [BsonElement("login")]
        public string Login { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("equipments")]
        public List<string> Equipments { get; set; }
        [BsonElement("enabled")]
        public bool Enabled { get; set; }
        [BsonElement("img")]
        public Image img { get; set; }
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

        public static bool ValidateNewMCU(MicroController device)
        {
            return true;
        }


        public static MicroController ValidateMCUUpdate(MicroController device)
        {
            return device;
        }


    }
}

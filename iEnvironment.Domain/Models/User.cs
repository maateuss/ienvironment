using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace iEnvironment.Domain.Models
{
    [BsonIgnoreExtraElements]
    public class User : BsonObject
    {
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("login")]
        public string Login { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("enabled")]
        public bool Enabled { get; set; }
        [BsonElement("img")]
        public Image img { get; set; }
    }
}

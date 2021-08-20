using iEnvironment.Domain.Enums;
using iEnvironment.Domain.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
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
        public Image Img { get; set; }

        public UserRole Role { get; set; }

        public bool ShouldSerializePassword() { return false; }
        public bool ShouldSerializeLogin() { return false; }

        public string GetClaimAttribute()
        {
            return Role.GetDescription();
        }

        public bool ValidateNewUser()
        {
            return true;
        }

        public User ValidateUserUpdate(User updatedUser)
        {
            Name = updatedUser.Name;
            Email = updatedUser.Email;
            Login = updatedUser.Login;
            Img = updatedUser.Img;
            Enabled = updatedUser.Enabled;
            Role = updatedUser.Role;
            return this;
        }
    }
}

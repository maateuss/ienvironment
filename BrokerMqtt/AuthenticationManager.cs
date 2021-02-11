using iEnvironment.Domain.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrokerMqtt
{
    public class AuthenticationManager
    {
        private static AuthenticationManager _instance;
        private static object locker = new object();
        private MongoClient mongoClient;
        private IMongoDatabase database;
        private IMongoCollection<User> collection;
        public string BaseUrl { get; set; }
        public AuthenticationManager(string baseUrl = "localhost")
        {
            BaseUrl = baseUrl;
        }
        public static AuthenticationManager GetInstance()
        {
            lock (locker)
            {
                if(_instance == null)
                {
                    _instance = new AuthenticationManager();
                    _instance.Build();
                }

                return _instance;
            }
        }
        public async Task<bool> ValidateUser(string username, string password) 
        { 
            return await GetInstance().validateUser(username, password);
        }

        private async Task<bool> validateUser(string username, string password)
        {
            var user = await collection.Find(x => x.Login == username).FirstOrDefaultAsync();

            if(user != null)
            {
                return BCrypt.Net.BCrypt.Verify(password, user.Password);
            }

            return false;

        }

        private void Build()
        {
            mongoClient = new MongoClient(BaseUrl);
            database = mongoClient.GetDatabase("ienvironment");
            collection = database.GetCollection<User>("users");
        }



    }
}

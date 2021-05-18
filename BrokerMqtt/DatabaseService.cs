using iEnvironment.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrokerMqtt
{
    public class DatabaseService
    {
        private MongoClient mongoClient;
        private IMongoDatabase database;
        private IMongoCollection<MicroController> collection;
        public string BaseUrl { get; set; }
        public DatabaseService(string BaseUrl = "localhost")
        {
            mongoClient = new MongoClient(BaseUrl);
            database = mongoClient.GetDatabase("ienvironmentV2");
            collection = database.GetCollection<MicroController>("microcontrollers");
        }



        public async Task<MicroController> ValidateUser(string username, string password)
        {
            var mcu = await collection.Find(x => x.Login == username).FirstOrDefaultAsync();

            if(mcu != null)
            {
                var result =  BCrypt.Net.BCrypt.Verify(password, mcu.Password);
                if(result)
                    return mcu;
                
            }

            return null;

        }


        public async Task ChangeConnectionStatus(MicroController microController, bool ConnectionStatus)
        {
            var mcu = await collection.Find(x => x.Id == microController.Id).FirstOrDefaultAsync();

            mcu.Connected = ConnectionStatus;

            await collection.FindOneAndReplaceAsync(x => x.Id == mcu.Id, mcu);
        }

        public async Task Setup()
        {
            var update = new UpdateDefinitionBuilder<MicroController>().Set(x => x.Connected, false);
            
            await collection.UpdateManyAsync(x => true, update);
        } 

    }
}

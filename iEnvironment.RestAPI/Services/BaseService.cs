using iEnvironment.Domain.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Services
{
    public abstract class BaseService<T> where T : BsonObject
    {
        private MongoClient mongoClient;
        private IMongoDatabase database;
        private IMongoCollection<T> collection;
        public BaseService(string collectionName)
        {
            mongoClient = new MongoClient(Settings.MongoConnectionString);
            database = mongoClient.GetDatabase(Settings.Database);
            collection = database.GetCollection<T>(collectionName);
        }

        public async Task<T> FindByID(string id)
        {
            return await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}

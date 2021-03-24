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
        public IMongoCollection<T> Collection;
        public BaseService(string collectionName)
        {
            mongoClient = new MongoClient(Settings.MongoConnectionString);
            database = mongoClient.GetDatabase(Settings.Database);
            Collection = database.GetCollection<T>(collectionName);
        }

        public async Task<IEnumerable<T>> FindAll()
        {
            return await Collection.Find(x=>true).ToListAsync();
        }

        public async Task<T> FindByID(string id)
        {
            return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}

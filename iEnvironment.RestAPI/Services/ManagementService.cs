using System;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MongoDB.Driver;
namespace iEnvironment.RestAPI.Services
{
    public class ManagementService
    {
        private MongoClient mongoClient;
        private IMongoDatabase database;
        public IMongoCollection<ManagementModel> Collection;
        public ManagementService()
        {
            mongoClient = new MongoClient(Settings.MongoConnectionString);
            database = mongoClient.GetDatabase(Settings.Database);
            Collection = database.GetCollection<ManagementModel>("management");
        }

        public async Task<ManagementModel> FetchInfo()
        {
            var info =  await Collection.Find(x => true).FirstOrDefaultAsync();
            if (info == null) 
            {
                var insertedManagement = new ManagementModel { BackendUrl = "10.54.21.36" };
                await Collection.InsertOneAsync(insertedManagement);
                return insertedManagement;
            }

            return info;

        }


    }


    public class ManagementModel : BsonObject
    {
        public string BackendUrl { get; set; }
    }

}

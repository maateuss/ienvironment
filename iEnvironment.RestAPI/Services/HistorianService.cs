using System;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MongoDB.Driver;

namespace iEnvironment.RestAPI.Services
{
    public class HistorianService
    {
        private MongoClient mongoClient;
        private EnvironmentService environmentService;
        private IMongoDatabase database;
        public IMongoCollection<DataVariation> Collection;
        public HistorianService()
        {
            mongoClient = new MongoClient(Settings.MongoConnectionString);
            database = mongoClient.GetDatabase(Settings.Database);
            Collection = database.GetCollection<DataVariation>("historical");
        }

        public async Task Create(Sensor currentValue, string newValue, string message = null)
        {
            var environment = await environmentService.FindByID(currentValue.EnvironmentId ?? "");

            if (currentValue.UpdatedAt < DateTime.Now.AddMinutes(-1) || (string)currentValue.CurrentValue != newValue)
            {
                await Collection.InsertOneAsync(new DataVariation(currentValue.Id, newValue, currentValue.Name, environment.Name ?? "", message ?? ""));
            }
        }
    }
}

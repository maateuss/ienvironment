using System;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MongoDB.Driver;

namespace iEnvironment.Watchman
{
    public class DataManager : DatabaseContext
    {
        IMongoCollection<DataVariation> Collection;
        IMongoCollection<Environments> EnvironmentCollections;

        public DataManager(WorkerOptions workerOptions) : base(workerOptions)
        {
            Collection = database.GetCollection<DataVariation>("historical");
            EnvironmentCollections = database.GetCollection<Environments>("environments");
        }


        internal async Task Create(Equipment currentValue, string newValue, string message = null)
        {
            var environment = await EnvironmentCollections.Find(x => x.Id == (currentValue.EnvironmentId ?? "")).FirstOrDefaultAsync();
            
            await Collection.InsertOneAsync(new DataVariation(currentValue.Id, newValue, currentValue.Name, environment.Name ?? "", message ?? ""));
            
        }
    }
}

using System;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MongoDB.Driver;

namespace iEnvironment.Watchman
{
    public class DataManager : DatabaseContext
    {
        IMongoCollection<DataVariation> Collection;

        public DataManager(WorkerOptions workerOptions) : base(workerOptions)
        {
            Collection = database.GetCollection<DataVariation>("historical");
        }


        internal async Task Create(Equipment currentValue, string newValue)
        {
            if (currentValue.UpdatedAt < DateTime.Now.AddMinutes(-1) || (string)currentValue.CurrentValue != newValue)
            {
                await Collection.InsertOneAsync(new DataVariation(currentValue.Id, newValue));
            }
        }
    }
}

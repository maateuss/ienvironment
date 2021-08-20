using System;
using MongoDB.Driver;

namespace iEnvironment.Watchman
{
    public class DatabaseContext
    {
        private MongoClient mongoClient;
        internal IMongoDatabase database;
        private readonly WorkerOptions Settings;
        public DatabaseContext(WorkerOptions options)
        {
            Settings = options;
            mongoClient = new MongoClient(Settings.MongoConnectionString);
            database = mongoClient.GetDatabase(Settings.Database);
        }
    }
}

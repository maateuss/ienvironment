using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iEnvironment.RestAPI.Hangfire
{
    public static class HangfireConfiguration
    {
        public static void ConfigureHangfire(this IServiceCollection services)
        {
            var migrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new MigrateMongoMigrationStrategy() ,
                BackupStrategy = new CollectionMongoBackupStrategy()
            };
            var storageOptions = new MongoStorageOptions
            {
                MigrationOptions = migrationOptions
            };

            services.AddHangfire(x =>
                x.UseMongoStorage(Settings.MongoConnectionString, "Hangfire", storageOptions));


        }

        public static void UseHangfire(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }

        [JobDisplayName("EventProcessor Integrity Check")]
        public static void StartServices()
        {
            MqttListeningService mqttListeningService = new MqttListeningService();

            mqttListeningService.Start();
        }


       
    }
}

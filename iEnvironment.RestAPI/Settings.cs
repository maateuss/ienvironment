using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI
{
    public static class Settings
    {
        public static string MongoConnectionString { get => mongoConnString; }
        public static string Database { get => database; }
        private static string mongoConnString { get; set; }
        private static string database { get; set; }

        public static void Configure(IConfiguration configuration)
        {
            mongoConnString = configuration["MongoCS"];
            database = configuration["Database"];
        }
    }
}

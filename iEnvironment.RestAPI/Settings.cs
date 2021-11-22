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
        public static int WorkFactor { get => workFactor; }
        public static int RefreshTokerValidationHours { get => refreshTokerValidationHours; }
        public static string AccessKey { get => accessKey; }
        public static string AccessSecret { get => accessSecret; }
        public static string Bucket { get => bucket; }
        public static string MqttEndpoint { get => mqttEndpoint; set => mqttEndpoint = value; }
        public static int MqttPort { get => mqttPort; }
        public static string S3Prefix { get => s3prefix; }
        public static string Secret { get => secret; }
        private static string mongoConnString { get; set; }
        private static string database { get; set; }
        private static int workFactor { get; set; }
        private static int refreshTokerValidationHours { get; set; }
        private static string accessKey { get; set; }
        private static string s3prefix { get; set; }
        private static string accessSecret { get; set; }
        private static string bucket { get; set; }
        private static string mqttEndpoint { get; set; }
        private static int mqttPort { get; set; }
        private static string secret { get; set; }
        public static void Configure(IConfiguration configuration)
        {
            mongoConnString = configuration["MongoCS"];
            database = configuration["Database"];
            workFactor = int.Parse(configuration["WorkFactor"]);
            accessKey = configuration["AccessKey"];
            accessSecret = configuration["AccessSecret"];
            bucket = configuration["bucket"];
            s3prefix = configuration["s3prefix"];
            secret = configuration["secret"];
            mqttEndpoint = configuration["mqttEndpoint"];
            mqttPort = int.Parse(configuration["mqttPort"]);
            refreshTokerValidationHours  = int.Parse(configuration["refreshTokenValidationHours"]);
        }
    }
}

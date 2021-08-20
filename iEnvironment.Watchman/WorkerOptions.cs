using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.Watchman
{
    public  class WorkerOptions
    {
        public string MongoConnectionString { get; set; }
        public string Database { get; set; }
        public int WorkFactor { get; set; }
        public int RefreshTokerValidationHours { get; set; }
        public string AccessKey { get; set; }
        public string AccessSecret { get; set; }
        public string Bucket { get; set; }
        public string MqttEndpoint { get; set; }
        public int MqttPort { get; set; }
        public string S3Prefix { get; set; }
        public string Secret { get; set; }
       
    }
}

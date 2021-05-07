using System;
using Newtonsoft.Json;

namespace iEnvironment.Domain.Models
{
    public class FlowData
    {


        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

    }
}

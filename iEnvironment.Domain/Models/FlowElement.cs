using System;
using Newtonsoft.Json;

namespace iEnvironment.Domain.Models
{


    public class FlowElement
    {
        public bool IsLine { get { return !string.IsNullOrEmpty(Source) && !string.IsNullOrEmpty(Target); } }
        public bool IsBlock { get { return !IsLine; } }
        
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("position")]
        public FlowElementPosition Position { get; set; }

        [JsonProperty("data")]
        public FlowData Data { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

    }

}

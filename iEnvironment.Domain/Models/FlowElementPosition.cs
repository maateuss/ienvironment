using System;
using Newtonsoft.Json;

namespace iEnvironment.Domain.Models
{
    public class FlowElementPosition
    {
        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }
    }
}

using System;
namespace iEnvironment.Domain.Models
{
    public class Image : BsonObject
    {
        public string FileName { get; set; }
        public string AltName { get; set; }
        public double Size { get; set; }
        public string Url { get; set; }

    }
}

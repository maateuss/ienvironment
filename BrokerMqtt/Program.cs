using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace BrokerMqtt
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", false, true);

            var config = builder.Build();
            Console.WriteLine("Hello World!");

        }
    }
}
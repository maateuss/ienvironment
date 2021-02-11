using iEnvironment.Domain.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BrokerMqtt
{
    class Program
    {
        private static bool IsRunning;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", false, true);


            var config = builder.Build();
            MqttBroker broker = new MqttBroker(int.Parse(config["port"]), config["MongoCS"]);

        
            broker.Start();

            IsRunning = true;


            while (IsRunning)
            {
                Thread.Sleep(0);
            }

            Console.WriteLine("Finalizando os trabsons");

        
            //var mongoClient = new MongoClient();
            //var database = mongoClient.GetDatabase("ienvironment");
            //var collection = database.GetCollection<User>("users");
           
            
            
            //var listOfAllUsers = collection.Find<User>(bunda => true).ToList();
            //var fabiaoViado = listOfAllUsers.First();
            //if(BCrypt.Net.BCrypt.Verify("123456", fabiaoViado.Password))
            //{
            //    Console.WriteLine("true");
            //}
            //else
            //{
            //    Console.WriteLine("false");
            //}

        }

    }
}
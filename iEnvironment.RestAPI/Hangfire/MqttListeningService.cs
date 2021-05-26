using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using iEnvironment.RestAPI.Services;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;

namespace iEnvironment.RestAPI.Hangfire
{
    public class MqttListeningService
    {
        public static IMqttClientOptions options;
        public static IMqttClient mqttClient;
        public List<(string Topic, bool Connected)> TopicsDict = new List<(string, bool)>();
        public MqttListeningService()
        {
            if (options == null)
            {
                options = new MqttClientOptionsBuilder()
                  .WithClientId("EventProcessor")
                  .WithTcpServer(Settings.MqttEndpoint, Settings.MqttPort)
                  .WithCredentials("atmega2560", "123456")
                  .WithCleanSession()
                  .Build();
            }

            if (mqttClient == null)
            {
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();
                mqttClient.UseDisconnectedHandler(async e => await DisconnectedHandler(e));
            }

        }

        public async void Start()
        {
            if (!mqttClient.IsConnected)
            {
                await Connect();
            }

            if (!TopicsDict.Any())
            {
                GetTopics();
            }

            if (TopicsDict.Any(x => !x.Connected))
            {
                Resubscribe();
            }


        }

        private void Resubscribe()
        {
            var topicList = TopicsDict.Select(x => x.Topic);
            var topicsToSubscribe = new List<MqttTopicFilter>();
            foreach (var item in topicList)
            {
                topicsToSubscribe.Add(new MqttTopicFilter { Topic = item });
            }

            var results = mqttClient.SubscribeAsync(topicsToSubscribe.ToArray()).Result;
            TopicsDict = new List<(string Topic, bool Connected)>();
            foreach (var item in results.Items)
            {
                TopicsDict.Add((item.TopicFilter.Topic, item.ResultCode > MqttClientSubscribeResultCode.GrantedQoS2));
            }

        }



        private void GetTopics()
        {
            TopicsDict = new List<(string Topic, bool Connected)>();
            TopicsDict.Add(("mgmt/", false));
            TopicsDict.Add(("sinal/#", false));
        }

        private async Task DisconnectedHandler(MqttClientDisconnectedEventArgs arg)
        {
            Console.WriteLine("### DISCONNECTED FROM SERVER ###");
            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                await mqttClient.ConnectAsync(options, CancellationToken.None);
            }
            catch
            {
                Console.WriteLine("### RECONNECTING FAILED ###");
            }
        }

        public async Task Connect()
        {
            try
            {
                await mqttClient.ConnectAsync(options, CancellationToken.None);
                if (mqttClient.IsConnected)
                {
                    Console.WriteLine($"Mqtt Connected @{DateTime.Now}");

                    mqttClient.UseApplicationMessageReceivedHandler(e =>
                    {
                        var payload = "";

                        if (e.ApplicationMessage.Payload != null)
                        {
                            payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                        }
                        MessageProcessorService.AddMessage(new Domain.Models.Message { Payload = payload, Topic = e.ApplicationMessage.Topic });
                        Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                        Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                        
                        Console.WriteLine($"+ Payload = {payload}");
                        Console.WriteLine();

                    });
                }
            }
            catch
            {
                Console.WriteLine("Not connected");
            }

        }

    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;

namespace iEnvironment.Watchman
{
    public class MessageProcessor
    {
        bool queueShouldRun;
        ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        ConcurrentQueue<Message> concurrentQueue = new ConcurrentQueue<Message>();
        public static IMqttClientOptions options;
        public static IMqttClient mqttClient;
        public List<(string Topic, bool Connected)> TopicsDict = new List<(string, bool)>();
        public bool Status { get => mqttClient?.IsConnected ?? false; }

        public MessageProcessor(string brokerUrl, int? port)
        {
            if (options == null)
            {
                options = new MqttClientOptionsBuilder()
                  .WithClientId("EventProcessor")
                  .WithTcpServer(brokerUrl, port)
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


            queueShouldRun = true;
            Task.Factory.StartNew(ProcessQueue);

            Start();
        }

        void AddMessage(Message message)
        {
            concurrentQueue.Enqueue(message);
            manualResetEvent.Set();
        }


        private void ProcessQueue()
        {
            while (queueShouldRun)
            {
                manualResetEvent.WaitOne();
                while (concurrentQueue.TryDequeue(out Message currentMessage))
                {
                    Console.WriteLine(currentMessage.Topic);
                    if (currentMessage.TryGetEquipmentId(out string equipmentid))
                    {
                        //UpdateSensor(currentMessage);
                    }
                }
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
                ResetTopics();
            }

            if (TopicsDict.Any(x => !x.Connected) && mqttClient.IsConnected)
            {
                Resubscribe();
            }


        }

        private void ResetTopics()
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

        async void UpdateSensor(Message currentMessage)
        {
            var sensorid = currentMessage.GetEquipmentId();
            try
            {
                Sensor sensor = await HardwareManager.FindByID(sensorid);

                sensor.KeepAlive = DateTime.Now;
                if ((string)sensor.CurrentValue != currentMessage.Payload)
                {
                    sensor.CurrentValue = currentMessage.Payload;
                }
                DataManager.Create(sensor, currentMessage.Payload);
                sensor.UpdatedAt = DateTime.Now;

                await HardwareManager.Update(sensorid, sensor);
            }
            catch
            {
                Console.WriteLine("Error updating sensor " + sensorid);
            }
        }

        async Task Connect()
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
                        AddMessage(new Message { Payload = payload, Topic = e.ApplicationMessage.Topic });
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
                TopicsDict.Add((item.TopicFilter.Topic, item.ResultCode < MqttClientSubscribeResultCode.GrantedQoS2));
            }
        }


    }
}

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
        bool timeBasedShouldRun; 
        ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        ConcurrentQueue<Message> concurrentQueue = new ConcurrentQueue<Message>();
        public static IMqttClientOptions options;
        public static IMqttClient mqttClient;
        public List<(string Topic, bool Connected)> TopicsDict = new List<(string, bool)>();
        public bool Status { get => mqttClient?.IsConnected ?? false; }
        private DataManager dataManager;
        private HardwareManager HardwareManager;
        private EventManager eventManager;
        private ActuatorManager actuatorManager;
        public MessageProcessor(WorkerOptions settings)
        {
            if (options == null)
            {
                options = new MqttClientOptionsBuilder()
                  .WithClientId("EventProcessor")
                  .WithTcpServer(settings.MqttEndpoint,settings.MqttPort)
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
            timeBasedShouldRun = true;

            dataManager = new DataManager(settings);
            HardwareManager = new HardwareManager(settings);
            eventManager = new EventManager(settings);
            actuatorManager = new ActuatorManager(settings);

            Task.Factory.StartNew(ProcessQueue);
            Task.Factory.StartNew(ProcessTimeBasedEvent);

          
            Start();
        }

        void AddMessage(Message message)
        {
            concurrentQueue.Enqueue(message);
            manualResetEvent.Set();
        }

        private async void ProcessTimeBasedEvent()
        {
            while (timeBasedShouldRun)
            {
                var listOfEvents = await eventManager.GetTimeBasedEvents();


                var listOfTopics = new List<(string topic, string payload)>();
             
                



                foreach (var item in listOfEvents)
                {
                    foreach (var acts in item.WhatExecute)
                    {
                        var topic = await actuatorManager.GetTopicById(acts.ActuatorId);
                        listOfTopics.Add((topic, acts.Value));
                    }

                }


                foreach (var item in listOfTopics)
                {
                    await mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                               .WithTopic($"{item.topic}")
                               .WithPayload($"{item.payload}")
                               .Build());
                }

           


                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
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
                        if (currentMessage.IsSensor)
                            UpdateSensor(currentMessage);
                        else if (currentMessage.IsActuator)
                            UpdateActuator(currentMessage);
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
            TopicsDict.Add(("action/#", false));
        }


        private async Task DisconnectedHandler(MqttClientDisconnectedEventArgs arg)
        {
            Console.WriteLine("### DISCONNECTED FROM SERVER ###");
            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                var result = await mqttClient.ConnectAsync(options, CancellationToken.None);
            }
            catch
            {
                Console.WriteLine("### RECONNECTING FAILED ###");
            }
        }

        async void UpdateSensor(Message currentMessage)
        {
            var sensorid = currentMessage.GetEquipmentId();
            string customMessage = "";
            try
            {
                if(sensorid.Length != 24) { Console.WriteLine("invalid eqpid"); return; }

                Sensor sensor = await HardwareManager.FindSensorById(sensorid);
                if(sensor == null)
                {
                    Console.WriteLine($"eqp {sensorid} not found");
                    return;
                }
                
                if (sensor.DefaultTriggersActive && int.TryParse(currentMessage.Payload, out int value))
                {
                    var lowerTrigger = Convert.ToInt32(sensor.LimitDown);
                    var upperTrigger = Convert.ToInt32(sensor.LimitUp);
                    if(upperTrigger > lowerTrigger )
                    {
                        if (value > upperTrigger)
                        {
                            customMessage = $"{value} recebido, acima do limite superior configurado de {upperTrigger}";
                            await mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                                .WithTopic($"alert/upperlimit/eqp/{sensorid}")
                                .WithPayload($"{value} received, over {upperTrigger} constraint")
                                .Build());
                        }
                        else if(value < lowerTrigger)
                        {
                            customMessage = $"{value} recebido, abaixo do limite inferior configurado de {lowerTrigger}";
                            await mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                                .WithTopic($"alert/lowerlimit/eqp/{sensorid}")
                                .WithPayload($"{value} received, under {lowerTrigger} constraint")
                                .Build());
                        }
                    }
                }

                sensor.KeepAlive = DateTime.Now;
                if ((string)sensor.CurrentValue != currentMessage.Payload)
                {
                    sensor.CurrentValue = currentMessage.Payload;
                }
                dataManager.Create(sensor, currentMessage.Payload, customMessage);
                sensor.UpdatedAt = DateTime.Now;

                await HardwareManager.Update(sensorid, sensor);
            }
            catch
            {
                Console.WriteLine("Error updating sensor " + sensorid);
            }
        }

        async void UpdateActuator(Message currentMessage)
        {
            var actuatorId = currentMessage.GetEquipmentId();
            try
            {
                if (actuatorId.Length != 24) { Console.WriteLine("invalid eqpid"); return; }

                Actuator actuator = await HardwareManager.FindActuatorById(actuatorId);
                if (actuator == null)
                {
                    Console.WriteLine($"eqp {actuatorId} not found");
                    return;
                }

     

                actuator.KeepAlive = DateTime.Now;
                if ((string)actuator.CurrentValue != currentMessage.Payload)
                {
                    actuator.CurrentValue = currentMessage.Payload;
                }
                dataManager.Create(actuator, currentMessage.Payload);
                actuator.UpdatedAt = DateTime.Now;

                await HardwareManager.UpdateActuador(actuatorId, actuator);
            }
            catch
            {
                Console.WriteLine("Error updating actuator " + actuatorId);
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

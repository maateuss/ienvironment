using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace iEnvironment.RestAPI.Services
{
    public class MqttService
    {
        public static IMqttClientOptions options;
        public static IMqttClient mqttClient;
        public MqttService()
        {
            if (options == null)
            {
                options = new MqttClientOptionsBuilder()
                  .WithClientId("ApiManager")
                  .WithTcpServer(Settings.MqttEndpoint, Settings.MqttPort)
                  .WithCredentials("atmega2560", "123456")
                  .WithCleanSession()
                  .Build();
            }

            if (mqttClient == null)
            {
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();
            }
        }


        public async Task<(bool result, string message)> SendMessages(List<(string topic, string payload)> messages)
        {

            try
            {
                if(!mqttClient.IsConnected)
                await mqttClient.ConnectAsync(options);

                foreach (var item in messages)
                {
                    await mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                                .WithTopic(item.topic)
                                .WithPayload(item.payload)
                                .Build());
                }

            }
            catch (Exception)
            {

                return (false, "Não foi possível enviar as mensagens, verifique o status do broker mqtt");
            }

            return (true, "");
        }

    }
}

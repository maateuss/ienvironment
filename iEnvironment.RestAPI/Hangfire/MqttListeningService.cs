using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;

namespace iEnvironment.RestAPI.Hangfire
{
    public class MqttListeningService
    {
        public static IMqttClientOptions options;
        public static IMqttClient mqttClient;
        public MqttListeningService()
        {
            if (options == null)
            {
                options = new MqttClientOptionsBuilder()
                  .WithClientId("EventProcessor")
                  .WithTcpServer(Settings.MqttEndpoint)
                  .WithCredentials("admin", "barreto")
                  .WithTls()
                  .WithCleanSession()
                  .Build();
            }

            if (mqttClient == null)
            {
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();
                mqttClient.UseDisconnectedHandler(async e => await DisconnectedHandler(e));
            }

            if (!mqttClient.IsConnected)
            {
                Task.Run(async () => { await Connect(); });
            }
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
            await mqttClient.ConnectAsync(options, CancellationToken.None);
            if (mqttClient.IsConnected)
                Console.WriteLine($"Mqtt Connected @{DateTime.Now}");
        }

    }
}

using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrokerMqtt
{
    public class MqttBroker
    {
        private int Port { get; set; }
        private DatabaseService DatabaseService { get; }
        private CacheService cache { get; set; }
        private ClientDisconnected BrokerEvents { get; set; }
        public MqttBroker(int port, string dbUrl)
        {
            Port = port;
            DatabaseService = new DatabaseService(dbUrl);
            cache = new CacheService();
            BrokerEvents = new ClientDisconnected();
            BrokerEvents.Disconnected += BrokerEvents_Disconnected;
        }

        private void BrokerEvents_Disconnected(object sender, MqttServerClientDisconnectedEventArgs args)
        {
            var mcu = cache.RegisterDisconnection(args.ClientId);
            DatabaseService.ChangeConnectionStatus(mcu, false);
        }

        public void Start()
        {

            DatabaseService.Setup();
            var optionsBuilder = new MqttServerOptionsBuilder().WithDefaultEndpoint().WithDefaultEndpointPort(Port).WithConnectionValidator(u =>
            {
                if(u.ClientId == "EventProcessor")
                {
                    u.ReasonCode = MqttConnectReasonCode.Success;
                    return;
                }

                if (u.ClientId.StartsWith("Mobile+"))
                {
                    u.ReasonCode = MqttConnectReasonCode.Success;
                    return;
                }


                if (String.IsNullOrWhiteSpace(u.Password) || String.IsNullOrWhiteSpace(u.Username))
                {
                    u.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    return;
                }
                else
                {
                    var microcontroller =  DatabaseService.ValidateUser(u.Username, u.Password).Result;
                    if (microcontroller == null)
                    {
                        u.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        return;
                    }
                    else
                    {
                        u.ReasonCode = MqttConnectReasonCode.Success;
                        cache.RegisterConnection(u.ClientId, microcontroller);
                        DatabaseService.ChangeConnectionStatus(microcontroller, true);
                    }
                }
            }).WithSubscriptionInterceptor(u =>
            {
                if (u.TopicFilter.ToString() == "invalido")
                {
                    u.AcceptSubscription = false;
                }
                u.AcceptSubscription = true;
                return;
            }).WithApplicationMessageInterceptor(u =>
            {
                if (u.ClientId.StartsWith("Mobile+"))
                {
                    u.AcceptPublish = false;
                    return;
                }
                if (u.ApplicationMessage.Payload != null)
                {
                    if (u.ApplicationMessage.Payload.Length > 200)
                    {
                        u.AcceptPublish = false;
                    }
                }
                u.AcceptPublish = true;
                Console.WriteLine($"{u.ApplicationMessage.ConvertPayloadToString()}");
                return;

            }); 
            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(optionsBuilder.Build());
            mqttServer.ClientDisconnectedHandler = BrokerEvents;
        }

    }
}

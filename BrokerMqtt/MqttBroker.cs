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
        private AuthenticationManager Authentication { get; }
        public MqttBroker(int port, string dbUrl)
        {
            Port = port;
            Authentication = new AuthenticationManager(dbUrl);
        }
        public void Start()
        {
            var optionsBuilder = new MqttServerOptionsBuilder().WithDefaultEndpoint().WithDefaultEndpointPort(Port).WithConnectionValidator(async u =>
            {
                
                if (String.IsNullOrWhiteSpace(u.Password) || String.IsNullOrWhiteSpace(u.Username))
                {
                    u.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    return;
                }
                else
                {
                    var valid = await Authentication.ValidateUser(u.Username, u.Password);
                    u.ReasonCode = valid ? MqttConnectReasonCode.Success : MqttConnectReasonCode.BadUserNameOrPassword;
                    
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
                if (u.ApplicationMessage.Payload.Length > 200)
                {
                    u.AcceptPublish = false;
                }
                u.AcceptPublish = true;
                return;

            });

            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(optionsBuilder.Build());
            
        }

    }
}

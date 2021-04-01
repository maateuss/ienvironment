using System;
using System.Threading.Tasks;
using MQTTnet.Server;

namespace BrokerMqtt
{
    public class ClientDisconnected : IMqttServerClientDisconnectedHandler
    {
        public delegate void NewDisconnection(object sender, MqttServerClientDisconnectedEventArgs args);
        public event NewDisconnection Disconnected;

        public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
        {
            Disconnected?.Invoke(this, eventArgs);
            return null;
        }
    }
}

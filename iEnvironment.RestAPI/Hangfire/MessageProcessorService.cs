using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using iEnvironment.RestAPI.Services;

namespace iEnvironment.RestAPI.Hangfire
{
    public static class MessageProcessorService
    {
        static ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        static ConcurrentQueue<Message> concurrentQueue = new ConcurrentQueue<Message>();
        static object UpdateSensorLock = new object();
        static SensorService sensorService;
        static HistorianService historianService;
        static MessageProcessorService()
        {
            Task.Factory.StartNew(ProcessMessages);
            sensorService = new SensorService();
            historianService = new HistorianService();
        }

        public static void AddMessage(Message message)
        {
            concurrentQueue.Enqueue(message);
            manualResetEvent.Set();
        }

        private static Task ProcessMessages()
        {
            while (true)
            {
                manualResetEvent.WaitOne();
                while (concurrentQueue.TryDequeue(out Message currentMessage))
                {
                    if (currentMessage.TryGetEquipmentId(out string equipmentid))
                    {
                        UpdateSensor(currentMessage);
                    }
                }
            }
        }

        private static async void UpdateSensor(Message currentMessage)
        {
            var sensorid = currentMessage.GetEquipmentId();
            try
            {
                var sensor = await sensorService.FindByID(sensorid);

                sensor.KeepAlive = DateTime.Now;
                if ((string)sensor.CurrentValue != currentMessage.Payload)
                {
                    sensor.CurrentValue = currentMessage.Payload;
                }
                historianService.Create(sensor, currentMessage.Payload);
                sensor.UpdatedAt = DateTime.Now;

                await sensorService.Update(sensorid, sensor);
            }
            catch
            {
                Console.WriteLine("Error updating sensor " + sensorid);
            }
        }
    }
}

using System;
using iEnvironment.Domain.Models;

namespace iEnvironment.RestAPI.Services
{
    public class SensorService : EquipmentService<Sensor>
    {
        public SensorService() : base("sensor")
        {
        }
    }
}

using System;
using iEnvironment.Domain.Models;

namespace iEnvironment.RestAPI.Services
{
    public class ActuatorService : EquipmentService<Actuator>
    {
        public ActuatorService() : base("actuator")
        {
        }
    }
}

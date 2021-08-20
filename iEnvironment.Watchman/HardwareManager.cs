using System;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;

namespace iEnvironment.Watchman
{
    public class HardwareManager
    {
        public HardwareManager()
        {
        }

        internal static Task Update(string sensorid, object sensor)
        {
            throw new NotImplementedException();
        }

        internal static Task<Sensor> FindByID(string sensorid)
        {
            throw new NotImplementedException();
        }
    }
}

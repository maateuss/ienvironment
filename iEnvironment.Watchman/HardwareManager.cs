using System;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MongoDB.Driver;

namespace iEnvironment.Watchman
{
    public class HardwareManager : DatabaseContext
    {

        IMongoCollection<Sensor> SensorCollection;
        IMongoCollection<Actuator> AtuadorCollection;

        public HardwareManager(WorkerOptions workerOptions) : base(workerOptions)
        {
            SensorCollection = database.GetCollection<Sensor>("sensor");
            AtuadorCollection = database.GetCollection<Actuator>("actuator");
        }

        internal async Task<bool> Update(string sensorid, Sensor sensor)
        {
            var currentEquipment = await SensorCollection.Find(x => x.Id == sensorid).FirstOrDefaultAsync();

            if (currentEquipment == null)
            {
                return false;
            }

            Equipment equipmentToUpdate = sensor.ValidateUpdate();

            if (equipmentToUpdate != null)
            {
                await SensorCollection.FindOneAndReplaceAsync(x => x.Id == sensorid, equipmentToUpdate as Sensor);
            }


            return true;
        }

        internal async Task<bool> UpdateActuador(string actuadorId, Actuator actuatur)
        {
            var currentEquipment = await AtuadorCollection.Find(x => x.Id == actuadorId).FirstOrDefaultAsync();

            if (currentEquipment == null)
            {
                return false;
            }

            Equipment equipmentToUpdate = actuatur.ValidateUpdate();

            if (equipmentToUpdate != null)
            {
                await AtuadorCollection.FindOneAndReplaceAsync(x => x.Id == actuadorId, equipmentToUpdate as Actuator);
            }


            return true;
        }



        internal async Task<Sensor> FindSensorById(string sensorid)
        {
            return await SensorCollection.Find(x => x.Id == sensorid).FirstOrDefaultAsync();
        }

        internal async Task<Actuator> FindActuatorById(string actuatorid)
        {
            return await AtuadorCollection.Find(x => x.Id == actuatorid).FirstOrDefaultAsync();
        }

    }
}

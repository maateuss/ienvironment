using System;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MongoDB.Driver;

namespace iEnvironment.Watchman
{
    public class HardwareManager : DatabaseContext
    {

        IMongoCollection<Sensor> Collection;

        public HardwareManager(WorkerOptions workerOptions) : base(workerOptions)
        {
            Collection = database.GetCollection<Sensor>("sensor");
        }

        internal async Task<bool> Update(string sensorid, Sensor sensor)
        {
            var currentEquipment = await Collection.Find(x => x.Id == sensorid).FirstOrDefaultAsync();

            if (currentEquipment == null)
            {
                return false;
            }

            Equipment equipmentToUpdate = sensor.ValidateUpdate();

            if (equipmentToUpdate != null)
            {
                await Collection.FindOneAndReplaceAsync(x => x.Id == sensorid, equipmentToUpdate as Sensor);
            }


            return true;
        }

        internal async Task<Sensor> FindByID(string sensorid)
        {
            return await Collection.Find(x => x.Id == sensorid).FirstOrDefaultAsync();
        }
    }
}

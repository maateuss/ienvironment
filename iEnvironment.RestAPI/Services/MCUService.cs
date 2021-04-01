using iEnvironment.Domain.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Services
{
    public class MCUService : BaseService<MicroController>
    {
        public MCUService() : base("microcontrollers")
        {


        }
        public async Task<bool> CreateNew(MicroController device)
        {
            if (device.Id != null)
            {
                var duplicate = await Collection.Find(x => x.Id == device.Id).FirstOrDefaultAsync();
                if(duplicate != null)
                {
                    return false;
                }
            }


            var loginExists = Collection.Find(x => x.Login == device.Login).Any();

            if (loginExists)
            {
                return false;
            }


            var isValid = MicroController.ValidateNewMCU(device);
            if (!isValid)
            {
                return false;
            }

            device.Password = CryptoService.HashPassword(device.Password);

            Collection.InsertOne(device);
            return true;
        }


        public async Task<bool> RemoveEquipmentReference(string id)
        {
            var MicroControllers = await Collection.Find(x => x.Equipments.Any(y => y.Contains(id))).ToListAsync();
            foreach (var item in MicroControllers)
            {
                item.RemoveEquipment(id);
                await Collection.FindOneAndReplaceAsync(x => x.Id == item.Id, item);
            }
            return true;
        }

        public async Task<bool> Update(string id, MicroController device)
        {
            var currentMicroController = await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (currentMicroController == null) return false;

            var validUpdate = MicroController.ValidateMCUUpdate(device);
            if (validUpdate != null)
            {
                await Collection.FindOneAndReplaceAsync(x => x.Id == id, validUpdate);
                return true;
            }

            return false;
        }

        internal async  Task<bool> Delete(string id)
        {
            var MCU = await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (MCU == null) return false;

            await Collection.FindOneAndDeleteAsync(x => x.Id == id);

            return true;
        }

        internal async Task UpdateEquipmentReference(string oldId, string newId, string eqpId)
        {
            var oldMCU = await Collection.Find(x => x.Id == oldId).FirstOrDefaultAsync();
            oldMCU.RemoveEquipment(eqpId);
            Collection.FindOneAndReplace(x => x.Id == oldId, oldMCU);

            var newMCU = await Collection.Find(x => x.Id == newId).FirstOrDefaultAsync();
            newMCU.AddEquipment(eqpId);
            Collection.FindOneAndReplace(x => x.Id == newId, newMCU);
        }

        internal async Task AddEquipmentReference(string microControllerId, string EquipmentId)
        {
            var newMCU = await Collection.Find(x => x.Id == microControllerId).FirstOrDefaultAsync();
            newMCU.AddEquipment(EquipmentId);
            Collection.FindOneAndReplace(x => x.Id == microControllerId, newMCU);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MongoDB.Driver;

namespace iEnvironment.RestAPI.Services
{
    public class EquipmentService<T> : BaseService<T> where T : Equipment
    {
        public EquipmentService (string entityName) : base(entityName)
        {

        }

        public async Task<T> Create(T equipment)
        {
            var duplicate = await Collection.Find(x => x.Id == equipment.Id).FirstOrDefaultAsync();
            if (duplicate != null)
            {
                return null;

            }

            var valid = equipment.ValidateNew();

            if (valid)
            {
                Collection.InsertOne(equipment);
            }

            return await Collection.Find(x =>x.Id == equipment.Id).FirstOrDefaultAsync();
        }

        public async Task<bool> Update(string id, T equipment)
        {
            var currentEquipment = await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if(currentEquipment == null)
            {
                return false;
            }

            var equipmentToUpdate = equipment.ValidateUpdate();


            return true;
        }

        public async Task<bool> Delete(string id)
        {
            await Collection.FindOneAndDeleteAsync(x => x.Id == id);
            return true;
        }

    }
}

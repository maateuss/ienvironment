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

            T equipmentToUpdate = (T) equipment.ValidateUpdate();

            if(equipmentToUpdate != null)
            {
                await Collection.FindOneAndReplaceAsync(x => x.Id == id, equipmentToUpdate);
            }
                       

            return true;
        }

      
    }
}

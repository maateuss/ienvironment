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

    }
}

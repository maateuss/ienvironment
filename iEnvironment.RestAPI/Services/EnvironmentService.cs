using iEnvironment.Domain.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Services
{
    public class EnvironmentService : BaseService<Environments>
    {
        public EnvironmentService() : base("environments")
        {

        }

        public async Task<bool> CreateNew(Environments env)
        {

            if (!String.IsNullOrWhiteSpace(env.Id))
            {
                var duplicate = Collection.Find(x => x.Id == env.Id).Any();
                if (duplicate)
                {
                    return false;
                }
            }
            if (!env.ValidateNewEnvironment())
            {
                return false;
            }

            Collection.InsertOne(env);

            return true;

        }

        public async Task<bool> RemoveEquipmentReference(string id)
        {
            var Environments = await Collection.Find(x => x.Equipments.Any(y => y.Contains(id))).ToListAsync();
            foreach (var item in Environments)
            {
                item.RemoveEquipment(id);
                await Collection.FindOneAndReplaceAsync(x => x.Id == item.Id, item);
            }
            return true;
        }


        public async Task<bool> EditEnvironment(string id, Environments env)
        {
            var currentEnvironment = await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (currentEnvironment == null)
            {
                return false;
            }

            var validEnvironment = env.ValidateEnvironmentUpdate();

            if (validEnvironment == null)
            {
                return false;
            }

            await Collection.FindOneAndReplaceAsync(x => x.Id == id, validEnvironment);
            return true;
        }

    }
}

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

            currentEnvironment.Name = env.Name ?? currentEnvironment.Name;
            currentEnvironment.Description = env.Description ?? currentEnvironment.Description;
            currentEnvironment.Enabled = env.Enabled;

      
            await Collection.FindOneAndReplaceAsync(x => x.Id == id, currentEnvironment);
            return true;
        }

        internal async Task AddEquipmentReference(string environmentId, string EquipmentId)
        {
            var environment = await Collection.Find(x => x.Id == environmentId).FirstOrDefaultAsync();
            environment.AddEquipment(EquipmentId);
            Collection.FindOneAndReplace(x => x.Id == environmentId, environment);
        }

        internal async Task RemoveEquipmentReference(string environmentId, string EquipmentId)
        {
            var environment = await Collection.Find(x => x.Id == environmentId).FirstOrDefaultAsync();
            environment.RemoveEquipment(EquipmentId);
            Collection.FindOneAndReplace(x => x.Id == environmentId, environment);
        }


        internal async Task AddEventReference(string environmentId, string EventId)
        {
            var environment = await Collection.Find(x => x.Id == environmentId).FirstOrDefaultAsync();
            environment.AddEvent(EventId);
            Collection.FindOneAndReplace(x => x.Id == environmentId, environment);
        }

        internal async Task RemoveEventReference(string environmentId, string EventId)
        {
            var environment = await Collection.Find(x => x.Id == environmentId).FirstOrDefaultAsync();
            environment.RemoveEvent(EventId);
            Collection.FindOneAndReplace(x => x.Id == environmentId, environment);
        }



    }
}

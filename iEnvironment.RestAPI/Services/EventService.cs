using System;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MongoDB.Driver;

namespace iEnvironment.RestAPI.Services
{
    public class EventService : BaseService<EventDefinition>
    {
        public EventService() : base("events")
        {

        }

        public async Task<EventDefinition> Create(EventDefinition ed)
        {
            var duplicate = await Collection.Find(x => x.Id == ed.Id).FirstOrDefaultAsync();
            if (duplicate != null)
            {
                return null;

            }

            var valid = ed.ValidateNewEventDefinition();

            if (valid)
            {
                Collection.InsertOne(ed);
            }

            return await Collection.Find(x => x.Id == ed.Id).FirstOrDefaultAsync();
        }


        public async Task<bool> Update(EventDefinition ed)
        {
            var currentEventDefinition = await Collection.Find(x => x.Id == ed.Id).FirstOrDefaultAsync();

            if(currentEventDefinition  == null)
            {
                return false;
            }

            var valid = ed.ValidateEventDefinitionUpdate();

            if(valid != null)
            {
                await Collection.FindOneAndReplaceAsync(x => x.Id == ed.Id, valid);
            }

            return true;
        }

        

    }
}

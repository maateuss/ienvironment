using System;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MongoDB.Driver;

namespace iEnvironment.RestAPI.Services
{
    public class EventService : BaseService<Event>
    {
        public EventService() : base("events")
        {

        }

        public async Task<Event> Create(Event ed)
        {
            var duplicate = await Collection.Find(x => x.Id == ed.Id).FirstOrDefaultAsync();
            if (duplicate != null)
            {
                return null;

            }

                Collection.InsertOne(ed);

            return await Collection.Find(x => x.Id == ed.Id).FirstOrDefaultAsync();
        }


        public async Task<bool> Update(Event ed)
        {

            var currentEventDefinition = await Collection.Find(x => x.Id == ed.Id).FirstOrDefaultAsync();

            if(currentEventDefinition  == null)
            {
                return false;
            }


            if (ed.Name != null)
            {
                currentEventDefinition.Name = ed.Name;
            }

            currentEventDefinition.Description = ed.Description != null ? ed.Description : currentEventDefinition.Description;
            


            var valid = ed.ValidateEventDefinitionUpdate(currentEventDefinition);

            if(valid != null)
            {
                await Collection.FindOneAndReplaceAsync(x => x.Id == ed.Id, valid);
            }

            return true;
        }

        

    }
}

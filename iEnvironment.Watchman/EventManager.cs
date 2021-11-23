using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEnvironment.Domain.Models;
using MongoDB.Driver;

namespace iEnvironment.Watchman
{
    public class EventManager : DatabaseContext
    {
        IMongoCollection<Event> Collection;

        public EventManager(WorkerOptions workerOptions) : base(workerOptions)
        {
            Collection = database.GetCollection<Event>("events");
        }

        public async Task<List<Event>> GetTimeBasedEvents()
        {
            var list = new List<Event>();

            var filter = Builders<Event>.Filter.Eq(x => x.TimeBased, true);

            var elements = await Collection.Find(filter).ToListAsync() ;
		
            foreach (var item in elements)
            {
                if (item.ShouldRun(DateTime.Now.AddHours(-3)))
                {
		    Console.WriteLine(item.ToString());
                    list.Add(item);
                }
            }

            return list;
        }

        public async Task<bool> Update(Event ed)
        {
            var currentEventDefinition = await Collection.Find(x => x.Id == ed.Id).FirstOrDefaultAsync();

            if (currentEventDefinition == null)
            {
                return false;
            }

            var valid = ed.ValidateEventDefinitionUpdate(currentEventDefinition);

            if (valid != null)
            {
                await Collection.FindOneAndReplaceAsync(x => x.Id == ed.Id, valid);
            }

            return true;
        }



    }

    public class ActuatorManager : DatabaseContext
    {
        IMongoCollection<Actuator> Collection;

        public ActuatorManager(WorkerOptions workerOptions) : base(workerOptions)
        {
            Collection = database.GetCollection<Actuator>("actuator");
        }

        public async Task<string> GetTopicById(string id)
        {
            var element = await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();


            return element?.Topic ?? "";
        }

    }

}
    

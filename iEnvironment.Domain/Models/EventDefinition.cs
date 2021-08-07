using System;
using System.Collections.Generic;

namespace iEnvironment.Domain.Models
{
    public class EventDefinition : BsonObject
    {
        public string CreatorID { get; set;}
        public string LastUpdateUserID { get; set; }
        public string EnvironmentID { get; set; }
        public int CoolDownSeconds { get; set; }
        public bool IsManual { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public List<FlowElement> NodeElements { get; set; }

        public bool ValidateNewEventDefinition()
        {
            return true;
        }

        public EventDefinition ValidateEventDefinitionUpdate()
        {
            return this;
        }

    }
}

using System;
using System.Collections.Generic;

namespace iEnvironment.Domain.Models
{
    public class Event : BsonObject
    {
        public string Name { get; set; }
        public string EnvironmentID { get; set; }
        public int CoolDownSeconds { get; set; }
        public bool IsManual { get; set; }
        public string Description { get; set; }
        public List<DayOfWeek> RunningDays { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool Enabled { get; set; }
        public bool TimeBased { get; set; }
        public List<Conditions> WhenExecute { get; set; }
        public List<Actions> WhatExecute { get; set; }

        public Event ValidateEventDefinitionUpdate(Event oldEvent)
        {
            this.Id = oldEvent.Id;
            this.CreatedAt = oldEvent.CreatedAt;
            this.UpdatedAt = DateTime.Now;


            return this;
        }
    }

    public class Actions
    {
        public string ActuatorId { get; set; }
        public string Value { get; set; }
    }

    public class Conditions
    {
        public string SensorId { get; set; }
        public string Value { get; set; }
        public ComparatorType Comparator { get; set; }
        public NextConditionType NextCondition { get; set; }
    }

    public enum ComparatorType
    {
        Equals = 0,
        GreaterThan = 1,
        LessThan = 2,
        DifferentFrom = 3,
        EqualOrLessThan = 4,
        EqualOrGreaterThan = 5
    }

    public enum NextConditionType
    {
        And = 0,
        Or = 1
    }
}

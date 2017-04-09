using System;

namespace ChopSuey.Model
{
    public class AggregateQuery
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Streak { get; set; }
        public string Init { get; set; }
        public string Aggregate { get; set; }
        public string Description { get; set; }
    }
}
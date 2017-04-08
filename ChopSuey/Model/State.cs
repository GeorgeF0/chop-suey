using System.Collections.Generic;

namespace ChopSuey.Model
{
    public interface IState
    {
        List<AggregateQueryRunner> Queries { get; }
    }

    public class State : IState
    {
        public List<AggregateQueryRunner> Queries { get; } = new List<AggregateQueryRunner>();
    }
}

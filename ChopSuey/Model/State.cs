using System.Collections.Generic;

namespace ChopSuey.Model
{
    public interface IState
    {
        List<AggregateQuery> Queries { get; }
    }

    public class State : IState
    {
        public List<AggregateQuery> Queries { get; } = new List<AggregateQuery>();
    }
}

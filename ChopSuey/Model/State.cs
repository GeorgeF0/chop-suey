using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Streak.Core;

namespace ChopSuey.Model
{
    public interface IState
    {
        List<QuerySummary> GetSummaries();
        void AddQuery(AggregateQuery query);
        void DeleteQuery(string id);
    }

    public class State : IState
    {
        private readonly object _sync = new object();
        private readonly Streak.Core.Streak _streak = new Streak.Core.Streak("data", writer: true);
        private readonly List<AggregateQueryRunner> _queries = new List<AggregateQueryRunner>();

        public State()
        {
            LoadQueries();
        }

        public List<QuerySummary> GetSummaries()
        {
            lock (_sync)
            {
                return _queries.Select(x => new QuerySummary
                {
                    Hits = x.Hits,
                    Errors = x.Errors,
                    State = x.State,
                    Query = x.Query
                }).ToList();
            }
        }

        public void AddQuery(AggregateQuery query)
        {
            lock (_sync)
            {
                var runner = new AggregateQueryRunner(query);

                _queries.Add(runner);

                var e = new Event
                {
                    Data = JsonConvert.SerializeObject(query),
                    Meta = "",
                    Type = "AddAggregateQuery"
                };

                _streak.Save(new[] { e });
            }
        }

        public void DeleteQuery(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("the query id cannot be null or empty");

            lock (_sync)
            {
                if (_queries.All(x => x.Query.Id != id)) throw new Exception($"there is no aggregate query with id {id}");
                var query = _queries.First(x => x.Query.Id == id);
                query.Dispose();
                _queries.Remove(query);

                var e = new Event
                {
                    Data = id,
                    Meta = "",
                    Type = "DeleteAggregateQuery"
                };

                _streak.Save(new[] { e });
            }
        }

        private void LoadQueries()
        {
            Console.WriteLine("Loading state...");

            var queries = new List<AggregateQuery>();

            foreach (var e in _streak.Get())
            {
                if (e.Type == "AddAggregateQuery") queries.Add(JsonConvert.DeserializeObject<AggregateQuery>(e.Data));
                if (e.Type == "DeleteAggregateQuery") queries.RemoveAll(x => x.Id == e.Data);
            }

            _queries.AddRange(queries.Select(x => new AggregateQueryRunner(x)).ToList());
        }
    }
}

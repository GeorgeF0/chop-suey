using System.Linq;
using ChopSuey.Model;
using Nancy;
using Nancy.ModelBinding;

namespace ChopSuey
{
    public class Api : NancyModule
    {
        public Api(IState state, IStorage storage) : base("/api")
        {
            Post["/query"] = _ =>
            {
                var query = this.Bind<AggregateQuery>();

                lock (state)
                {
                    var runner = new AggregateQueryRunner(query);
                    state.Queries.Add(runner);
                    storage.Save(query);
                    runner.Run();
                }

                return HttpStatusCode.Created;
            };

            Get["/query"] = _ =>
            {
                lock (state)
                {
                    return state.Queries.Select(x => new QuerySummary
                    {
                        Hits = x.Hits,
                        Errors = x.Errors,
                        Description = x.Query.Description,
                        State = x.State
                    });
                }
            };
        }
    }

    public class QuerySummary
    {
        public int Hits { get; set; }
        public int Errors { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
    }
}

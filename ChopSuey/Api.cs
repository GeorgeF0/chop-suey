using ChopSuey.Model;
using Nancy;
using Nancy.ModelBinding;

namespace ChopSuey
{
    public class Api : NancyModule
    {
        public Api(IState state) : base("/api")
        {
            Post["/query"] = _ =>
            {
                var query = this.Bind<QueryRequest>();

                lock (state)
                {
                    var aggregateQuery = new AggregateQuery(query.Streak, query.Init, query.Aggregate, query.Description);
                    state.Queries.Add(aggregateQuery);
                    aggregateQuery.Run();
                }

                return HttpStatusCode.Created;
            };
        }
    }

    public class QueryRequest
    {
        public string Streak { get; set; }
        public string Init { get; set; }
        public string Aggregate { get; set; }
        public string Description { get; set; }
    }
}

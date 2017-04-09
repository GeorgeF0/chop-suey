using ChopSuey.Model;
using Nancy;
using Nancy.ModelBinding;

namespace ChopSuey
{
    public class Api : NancyModule
    {
        public Api(IState state) : base("/api")
        {
            Get["/query"] = _ => state.GetSummaries();

            Post["/query"] = _ =>
            {
                var query = this.Bind<AggregateQuery>();

                state.AddQuery(query);

                return HttpStatusCode.Created;
            };

            Delete["/query/{id}"] = x =>
            {
                string id = x.id;

                state.DeleteQuery(id);

                return 200;
            };
        }
    }

    public class QuerySummary
    {
        public int Hits { get; set; }
        public int Errors { get; set; }
        public string State { get; set; }
        public AggregateQuery Query { get; set; }
    }
}

using ChopSuey.AggregateQueries;
using Nancy;
using Nancy.ModelBinding;

namespace ChopSuey.Nancy
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
}

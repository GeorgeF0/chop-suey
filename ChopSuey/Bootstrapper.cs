using System;
using System.Linq;
using ChopSuey.Model;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace ChopSuey
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("/", @"Content")
            );
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            Console.WriteLine("Loading state...");

            // Load saved state
            var storage = container.Resolve<IStorage>();
            var queries = storage.Load<AggregateQuery>();

            // Start runners
            var runners = queries.Select(x => new AggregateQueryRunner(x)).ToList();
            runners.ForEach(x => x.Run());

            // Add runners to state
            var state = container.Resolve<IState>();
            state.Queries.AddRange(runners);
        }
    }
}

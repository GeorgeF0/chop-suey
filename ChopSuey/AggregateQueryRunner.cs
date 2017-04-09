using System;
using System.Threading;
using System.Threading.Tasks;
using ChopSuey.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChopSuey
{
    public class AggregateQueryRunner : IDisposable
    {
        private readonly Streak.Core.Streak _streak;
        private readonly CancellationTokenSource _tokenSource;
        private dynamic _state;
        private readonly Aggregate _aggregate;

        public int Hits { get; private set; }
        public int Errors { get; private set; }
        public AggregateQuery Query { get; private set; }

        public string State
        {
            get
            {
                lock (_state)
                {
                    return JsonConvert.SerializeObject(_state, Formatting.Indented);
                }
            }
        }

        public AggregateQueryRunner(AggregateQuery query)
        {
            if (query.Streak == null) throw new Exception("Streak cannot be null!");
            if (query.Init == null) throw new Exception("Init function cannot be null!");
            if (query.Aggregate == null) throw new Exception("Aggregate function cannot be null!");

            Query = query;

            //Compile lambdas
            var init = LambdaCompiler.CreateInit(query.Init);
            _aggregate = LambdaCompiler.CreateLambda(query.Aggregate);

            //Create reader streak
            _streak = new Streak.Core.Streak(query.Streak);

            //Initialize state
            _state = init();

            //Run
            _tokenSource = new CancellationTokenSource();
            Run(_tokenSource.Token);
        }

        private void Run(CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var e in _streak.Get(continuous: true))
                {
                    if (token.IsCancellationRequested) return;

                    try
                    {
                        dynamic data = JObject.Parse(e.Data);

                        lock (_state) _aggregate(e, data, ref _state);

                        Hits++;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        Errors++;
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
            _streak.Dispose();
        }
    }
}
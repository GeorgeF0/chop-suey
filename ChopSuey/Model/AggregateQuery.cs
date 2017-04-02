﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Streak.Core;

namespace ChopSuey.Model
{
    public class AggregateQuery
    {
        private readonly IEnumerable<Event> _streak;
        private dynamic _state;

        public int Hits { get; private set; }
        public int Errors { get; private set; }

        public Init Init { get; }
        public Aggregate Aggregate { get; }

        public string InitStr { get; private set; }
        public string AggregateStr { get; private set; }
        public string Description { get; private set; }

        public string State
        {
            get
            {
                lock (_state)
                {
                    return JsonConvert.SerializeObject(_state);
                }
            }
        }

        public AggregateQuery(string streak, string init, string aggregate, string description)
        {
            if (streak == null) throw new Exception("Streak cannot be null!");
            if (init == null) throw new Exception("Init function cannot be null!");
            if (aggregate == null) throw new Exception("Aggregate function cannot be null!");

            //Compile lambdas
            Init = LambdaCompiler.CreateInit(init);
            Aggregate = LambdaCompiler.CreateLambda(aggregate);

            InitStr = init;
            AggregateStr = aggregate;
            Description = description;

            //Create reader streak
            _streak = new Streak.Core.Streak(streak).Get(continuous: true);

            //Initialize state
            _state = Init();
        }

        public Task Run()
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (var e in _streak)
                {
                    try
                    {
                        dynamic data = JObject.Parse(e.Data);

                        lock (_state) Aggregate(e, data, _state);

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
    }

    public delegate dynamic Init();
    public delegate void Aggregate(Event evt, dynamic data, dynamic state);
}

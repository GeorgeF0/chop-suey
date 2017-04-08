using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Streak.Core;

namespace ChopSuey
{
    public interface IStorage
    {
        void Save(object item);
        List<T> Load<T>();
    }

    public class Storage : IStorage
    {
        private readonly Streak.Core.Streak _streak = new Streak.Core.Streak("data", writer: true);

        public void Save(object item)
        {
            lock (_streak)
            {
                var e = new Event
                {
                    Data = JsonConvert.SerializeObject(item),
                    Meta = "",
                    Type = item.GetType().Name
                };

                _streak.Save(new[] {e});
            }
        }

        public List<T> Load<T>() => 
            _streak.Get()
            .Where(x => x.Type == typeof(T).Name)
            .Select(x => JsonConvert.DeserializeObject<T>(x.Data))
            .ToList();
    }
}

using StackExchange.Redis;
using System;
using System.Text.Json;

namespace Catalog.API.Repositories
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public CacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public T GetData<T>(string key)
        {
            var _db = _connectionMultiplexer.GetDatabase(1);
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public object RemoveData(string key)
        {
            var _db = _connectionMultiplexer.GetDatabase(1);
            var value = _db.KeyExists(key);
            if (value)
            {
                return _db.KeyDelete(key);
            }
            return false;
        }

        public bool SetData<T>(string key, T value)
        {
            var db = _connectionMultiplexer.GetDatabase(1);
            var serial = JsonSerializer.Serialize(value);
            //var expiryTime = DateTimeOffset.Now.AddMinutes(Time);
            //var expiry = expiryTime.DateTime.Subtract(DateTime.Now);
            var set = db.StringSet(key, serial);
            return set;
        }
    }
}

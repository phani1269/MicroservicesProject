using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
            var _db = _connectionMultiplexer.GetDatabase(2);
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;

           
        }

        public object RemoveData(string key)
        {
            var _db = _connectionMultiplexer.GetDatabase(2);
            var value = _db.KeyExists(key);
            if (value)
            {
                return _db.KeyDelete(key);
            }
            return false;

        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var _db = _connectionMultiplexer.GetDatabase(2);
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var serial = JsonSerializer.Serialize(value);
            var isSet = _db.StringSet(key, serial, expiryTime);
            return isSet;
        }
    }
}

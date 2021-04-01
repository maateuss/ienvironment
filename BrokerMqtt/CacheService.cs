using System;
using System.Collections.Generic;
using iEnvironment.Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BrokerMqtt
{
    public class CacheService
    {
        private MemoryCache cache;
        private const string ListKey = "_activelist";

        public CacheService()
        {
            cache = new MemoryCache(new MemoryCacheOptions
            {
            });

            cache.Set(ListKey, new List<String>());
        }

        public void RegisterConnection(string clientid, MicroController MCU)
        {
            cache.Set(clientid, MCU);
            var list = cache.Get<List<string>>(ListKey);
            list.Add(MCU.Id);
            cache.Set(ListKey, list);
            
        }

        public MicroController RegisterDisconnection(string clientid)
        {
            var MCU = cache.Get<MicroController>(clientid);
            var list = cache.Get<List<string>>(ListKey);
            list.Remove(MCU.Id);
            cache.Set(ListKey, list);
            return MCU;
        } 

    }
}

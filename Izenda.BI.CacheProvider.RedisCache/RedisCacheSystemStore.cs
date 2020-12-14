using Izenda.BI.Cache.Contracts;
using Izenda.BI.Cache.Metadata.Constants;
using Izenda.BI.Core;
using Izenda.BI.Framework.Constants;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using System.Threading.Tasks;

namespace Izenda.BI.CacheProvider.RedisCache
{
    [Export(typeof(ICacheStore))]
    [ExportMetadata("CacheStore", "RedisCacheStore")]
    [ExportMetadata("CacheStoreType", CacheType.SystemCache)]
    public class RedisCacheSystemStore : RedisCacheStore
    {
        private readonly RedisCache redisCache;

        public RedisCacheSystemStore()
            : base(true)
        {
            redisCache = new RedisCache(IzendaJsonSerializerSettings.CacheSettings);
            this.SetTimeToLive(CacheConfiguration.Instance.CurrentSetting.SystemCacheTTL);
        }

        protected override RedisCache RedisCache => redisCache;

        public override CacheType CacheType => CacheType.SystemCache;

        public override string CacheDirectory => throw new NotImplementedException();

        public override Task ExecuteReloadCacheData(Dictionary<Guid, object> datasourceAdaptors, int loadDuration)
        {
            return Task.CompletedTask;
        }

        public override Task ExecuteRestoreFromMetadata(Dictionary<Guid, object> datasourceAdaptors)
        {
            return Task.CompletedTask;
        }

        protected override void SetTimeToLive(int timeToLive)
        {
            base.TimeToLive = Math.Max(timeToLive, MinSystemCacheTTL);
        }
    }
}

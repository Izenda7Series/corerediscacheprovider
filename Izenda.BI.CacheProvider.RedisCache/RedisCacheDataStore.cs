using Izenda.BI.Cache;
using Izenda.BI.Cache.Contracts;
using Izenda.BI.Cache.Metadata;
using Izenda.BI.Cache.Metadata.Constants;
using Izenda.BI.Core;
using Izenda.BI.DataAdaptor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Izenda.BI.CacheProvider.RedisCache
{
    [Export(typeof(ICacheStore))]
    [ExportMetadata("CacheStore", "RedisCacheStore")]
    [ExportMetadata("CacheStoreType", CacheType.DataCache)]
    public class RedisCacheDataStore : RedisCacheStore
    {
        private readonly RedisCache redisCache;

        public RedisCacheDataStore()
            : base(CacheConfiguration.Instance.CurrentSetting.IsEnableDataCache)
        {
            redisCache = new RedisCache(new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Unspecified,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> { new CacheDataConverter() }
            });

            this.SetTimeToLive(CacheConfiguration.Instance.CurrentSetting.DataCacheTTL);
        }

        protected override RedisCache RedisCache => redisCache;

        public override CacheType CacheType => CacheType.DataCache;

        public override string CacheDirectory => throw new NotImplementedException();

        public override Task ExecuteReloadCacheData(Dictionary<Guid, object> datasourceAdaptors, int loadDuration)
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(loadDuration * 1000);
            var cancellationToken = tokenSource.Token;

            return Task.Factory.StartNew(
                () => UpdateCacheFromMetadata(this.cacheMetadataItems, datasourceAdaptors, cancellationToken, keepCreatedDate: true),
                cancellationToken);
        }

        public override Task ExecuteRestoreFromMetadata(Dictionary<Guid, object> datasourceAdaptors)
        {
            return Task.Factory.StartNew(() =>
            {
                var metadataItems = Repository.GetIzendaCacheMetadata((int)CacheType);
                UpdateCacheFromMetadata(metadataItems, datasourceAdaptors, CancellationToken.None);
            });
        }

        private void UpdateCacheFromMetadata(List<IzendaCacheMetadata> metadataItems, Dictionary<Guid, object> datasourceAdaptors, CancellationToken cancellationToken, bool keepCreatedDate = false)
        {
            if (!metadataItems.Any())
                return;

            var validMetadata = metadataItems.Where(x => !x.IsExpired(TimeToLive, DateTime.UtcNow)).ToList();
            foreach (var item in validMetadata)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var metadata = QueryMetadata.ExtractQueryMetadata(item.CacheMetadata);
                metadata.CacheCreatedDate = keepCreatedDate ? item.CreatedDate : DateTime.UtcNow;
                metadata.IgnoreCache = true;
                var dataAdaptor = datasourceAdaptors[metadata.ServerType] as IDataSourceAdaptor;

                if (metadata.IsPaging)
                {
                    dataAdaptor.QueryFusionPagingResult(metadata, metadata.QueryTimeout);
                }
                else
                {
                    dataAdaptor.QueryFusionDataNoPaging(metadata, metadata.QueryTimeout);
                }
            }
        }

        protected override void SetTimeToLive(int timeToLive)
        {
            base.TimeToLive = Math.Max(timeToLive, MinDataCacheTTL);
        }
    }
}

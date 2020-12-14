using Izenda.BI.Utility;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Izenda.BI.CacheProvider.RedisCache.Utilities
{
    internal static class RedisHelper
    {
        private static readonly string redisCacheConnectionString;
        private static readonly string redisCacheAdditionalOptions;
        private static IConnectionMultiplexer connection = null;
        private static IDatabase database = null;
        private static IServer server = null;

        static RedisHelper()
        {
            redisCacheConnectionString = AppSettingsUtil.GetAppSettingEntry("izenda.cache.rediscache.connectionstring");
            redisCacheAdditionalOptions = AppSettingsUtil.GetAppSettingEntry("izenda.cache.rediscache.additionaloptions");
            connection = GetConnection();
            database = connection.GetDatabase();
        }

        public static IDatabase Database
        {
            get
            {
                if (database == null)
                {
                    connection.GetDatabase();
                }

                return database;
            }
        }

        public static IServer Server
        {
            get
            {
                if (server == null)
                {
                    server = connection.GetServer(redisCacheConnectionString);
                }

                return server;
            }
        }

        private static IConnectionMultiplexer GetConnection()
        {
            var redisConfiguration = redisCacheConnectionString;
            if (!string.IsNullOrWhiteSpace(redisCacheAdditionalOptions))
                redisConfiguration = $"{redisConfiguration},{redisCacheAdditionalOptions}";

            return ConnectionMultiplexer.Connect(redisConfiguration);
        }
    }
}

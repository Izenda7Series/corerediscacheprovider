# Izenda .NET Core RedisCacheProvider (v4.0.0)

## Overview
This is a custom cache provider that utilizes the Redis cache.  

:warning: The current version of this project will only work with Izenda's .NET Core Resources

## Installation

1. Publish the project and copy the DLLs to the Izenda API directory:
```
dotnet publish Izenda.BI.CacheProvider.RedisCache.csproj -c Release -o "C:\izenda" -f netcoreapp3.1
```
   
2. Remove the following DLL from the Izenda API directory:
   
   * Izenda.BI.CacheProvider.Memcache.dll

   :warning: If you do not remove the default Memcache provider referenced in this step, caching may not work properly.
   
3. Remove any references to Izenda.BI.CacheProvider.Memcache from the Izenda.BI.API.AspNetCore.deps.json file in the Izenda API directory

4. Update the following app settings in the appsettings.json file of the API instance as detailed below
```
	"izenda.cache.data.category": "Schedule",
	"izenda.cache.system.category": "Schedule",
	"izenda.cache.data.cachestore": "RedisCacheStore",
	"izenda.cache.system.cachestore": "RedisCacheStore",
```

5. Add the following app settings in the appsettings.json file of the API instance as detailed below while replacing the values to accommodate your environment
```
	"izenda.cache.rediscache.connectionstring": "10.111.5.132:6379",
	"izenda.cache.rediscache.additionaloptions": "abortConnect=false",
```	

6. Restart the API instance
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using WebAPINet6.BusinessLogic.Model;

namespace WebAPINet6.BusinessLogic.Services.Background
{
    public class CacheUpdater : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly Keys _keys;
        private readonly ILogger<CacheUpdater> _log;
        private readonly IMemoryCache _cache;

        public CacheUpdater(IServiceProvider provider, Keys keys, ILogger<CacheUpdater> logger, IMemoryCache cache) 
        {
            _provider = provider;
            _keys = keys;
            _log = logger;
            _cache = cache;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _provider.CreateScope()) 
                { 
                    var scopedProvider = scope.ServiceProvider;
                    var client = scope.ServiceProvider.GetRequiredService<Interfaces.IClient>();

                    var keys = new HashSet<string>();
                    lock (_keys)
                        keys = _keys.cacheKeys;

                    foreach (var key in keys)
                    {
                        var result = await client.GetSymbols(key);
                        
                        _cache.Set(key, result.First(), TimeSpan.FromSeconds(60));
                        _log.LogInformation("BACKGROUND TASK: In cache, updated id: {id}", key);
                    }
                }
                
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }
    }
}

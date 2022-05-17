using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebAPINet6.BusinessLogic.Model;
using WebAPINet6.BusinessLogic.Repository;
using WebAPINet6.WebApi;

namespace WebAPINet6.BusinessLogic.Services.Background
{
    public class CacheData : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly Keys _keys;
        private readonly ILogger<CacheData> _log;

        public CacheData(IServiceProvider provider, Keys keys, ILogger<CacheData> logger) 
        {
            _provider = provider;
            _keys = keys;
            _log = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _provider.CreateScope()) 
                { 
                    var scopedProvider = scope.ServiceProvider;
                    var client = scope.ServiceProvider.GetRequiredService<Services.Interfaces.IClient>();
                    IMemoryCache cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
                   
                    foreach (var key in _keys.cacheKeys)
                    {
                        var result = await client.GetSymbols(key);

                        cache.Set(key, result.First(), TimeSpan.FromSeconds(60));

                        _log.LogInformation("In cache, updated id: {id}", key);
                    }
                }
                
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}

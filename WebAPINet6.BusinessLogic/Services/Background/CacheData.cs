using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WebAPINet6.BusinessLogic.Model;
using WebAPINet6.BusinessLogic.Repository;
using WebAPINet6.WebApi;

namespace WebAPINet6.BusinessLogic.Services.Background
{
    public class CacheData : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly Keys _keys;

        public CacheData(IServiceProvider provider, Keys keys) 
        {
            _provider = provider;
            _keys = keys;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _provider.CreateScope()) 
                { 
                    var scopedProvider = scope.ServiceProvider;
                    var client = scope.ServiceProvider.GetRequiredService<IClient>();
                    IMemoryCache cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
                   
                    foreach (var key in _keys.cacheKeys)
                    {
                        var result = await client.GetSymbols(key);

                        cache.Set(key, result.First(), TimeSpan.FromSeconds(60));
                    }
                }
                
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }
    }
}

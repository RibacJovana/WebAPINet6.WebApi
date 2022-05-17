using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WebAPINet6.BusinessLogic.Model;
using WebAPINet6.BusinessLogic.Services.Interfaces;
using WebAPINet6.WebApi;

namespace WebAPINet6.BusinessLogic.Services
{
    public class CacheTaker : ICacheTaker
    {
        private readonly IMemoryCache _cache;
        private readonly Keys _keys;
        private readonly ILogger<CacheTaker> _logger;

        public CacheTaker(IMemoryCache cache, Keys keys, ILogger<CacheTaker> logger)
        {
            _cache = cache;
            _keys = keys;
            _logger = logger;
        }

        public async Task<List<SymbolInfo>> GetSymbolsInfo(string[]? ids)
        { 
            List<SymbolInfo> infoFromCache = new();

            foreach (string id in ids!)
            {
                await Task.Run(() =>
                {
                    // proveravamo da li id imamo vec u cache-u 
                    var isInCache = _cache.TryGetValue(id, out SymbolInfo symbolInfo);

                    if (!isInCache)
                    {
                        // ako id nije u cache, dodajemo ga u poseban niz radi evidencije
                        _keys.missingKeys.Add(id);
                    }
                    else
                    {
                        // ako id jeste u cache, zato sto je symbolInfo prosledjena kao OUT, imamo informacije koje je cache vratio
                        infoFromCache.Add(symbolInfo);

                        _logger.LogInformation("From cache took id: {id}", id);
                    }
                });
            }
            return infoFromCache;
        }

    }
}

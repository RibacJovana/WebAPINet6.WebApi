using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using WebAPINet6.WebApi;
using WebAPINet6.BusinessLogic.Model;
using WebAPINet6.BusinessLogic.Services.Background;
using WebAPINet6.BusinessLogic.Services.Interfaces;

namespace WebAPINet6.BusinessLogic.Services
{
    public class DataTaker : IDataTaker
    {
        private readonly IMemoryCache _cache;
        private readonly Keys _keys;
        private readonly ILogger<DataTaker> _logger;
        private readonly IClient _client;

        public DataTaker(IMemoryCache cache, Keys keys, ILogger<DataTaker> logger, IClient client)
        {
            _cache = cache;
            _keys = keys;
            _logger = logger;
            _client = client;
        }

        public async Task<List<SymbolInfo>> GetSymbolsInfo(string[] ids)
        {
            List<SymbolInfo> infoFromCache = new();
            List<SymbolInfo> infoFromClient = new();
            List<string> missingKeys = new();

            foreach (string id in ids)
            { 
                // proveravamo da li id imamo vec u cache-u 
                if (!_cache.TryGetValue(id, out SymbolInfo symbolInfo))
                {
                    missingKeys.Add(id);
                }
                else
                {
                    // ako id jeste u cache, zato sto je symbolInfo prosledjena kao OUT, imamo informacije koje je cache vratio
                    infoFromCache.Add(symbolInfo);

                    _logger.LogInformation("SERVICE: From cache took id: {id}", id);
                }
            }

            if (missingKeys.Count > 0)
            {
                infoFromClient = await _client.GetSymbols(string.Join(" ", missingKeys));

                // U globalnu staticku listu dodajemo sve ids koji nisu bili u cache, i vrednosti koje je client vration.
                // Ta lista nam sluzi da evidentiramo sve ids koji su u cache
                foreach (SymbolInfo? symbol in infoFromClient.GroupBy(x => x.Id).Select(x => x.FirstOrDefault()).Distinct())
                {
                    string? id = symbol!.Id;

                    _cache.Set(symbol.Id, symbol, TimeSpan.FromSeconds(CacheUpdater.TIMER_cache));
                    lock (_keys)
                        _keys.cacheKeys.Add(id);

                    _logger.LogInformation("SERVICE: In cache added id: {id}", id);
                }
            }

            return infoFromCache.Concat(infoFromClient).ToList();
        }
    }
}

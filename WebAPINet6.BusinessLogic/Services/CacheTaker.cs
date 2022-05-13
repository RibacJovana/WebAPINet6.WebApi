using Microsoft.Extensions.Caching.Memory;
using WebAPINet6.BusinessLogic.Model;
using WebAPINet6.WebApi;

namespace WebAPINet6.BusinessLogic.Services
{
    public class CacheTaker : ICacheTaker
    {
        private readonly IMemoryCache _cache;
        private readonly Keys _keys;
        public CacheTaker(IMemoryCache cache, Keys keys)
        {
            _cache = cache;
            _keys = keys;
        }

        public async Task<List<SymbolInfo>> GetSymbolsInfo(string[] ids)
        { 
            List<SymbolInfo> infoFromCache = new List<SymbolInfo>();

            foreach (var id in ids)
            {
                SymbolInfo symbolInfo;

                // proveravamo da li id imamo vec u cache-u 
                var isInCache = _cache.TryGetValue(id, out symbolInfo);

                if (!isInCache)
                {
                    // ako id nije u cache, dodajemo ga u poseban niz radi evidencije
                    _keys.missingKeys.Add(id);
                }
                else
                {
                    // ako id jeste u cache, zato sto je symbolInfo prosledjena kao OUT, imamo informacije koje je cache vratio
                    infoFromCache.Add(symbolInfo);
                }
            }

            return infoFromCache;
        }

    }
}

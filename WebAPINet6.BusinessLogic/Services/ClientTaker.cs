using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPINet6.BusinessLogic.Model;
using WebAPINet6.WebApi;

namespace WebAPINet6.BusinessLogic.Services 
{
     public class ClientTaker : IClientTaker
    {
        private readonly IMemoryCache _cache;
        private readonly IClient _symbol;
        private readonly Keys _keys;
        public ClientTaker(IMemoryCache cache, IClient symbol, Keys keys)
        {
            _cache = cache;
            _symbol = symbol;
            _keys = keys;
        }

        public async Task<List<SymbolInfo>> GetSymbolsInfo()
        {
            // pozivamo klijenta da nam vrati sve informacije za ids koji nisu u cache
            var infoFromClientForMissingIds = await _symbol.GetSymbols(string.Join(" ", _keys.missingKeys));
            
            // resetujemo missing keys
            _keys.missingKeys.Clear();

            // U globalnu staticku listu dodajemo sve ids koji nisu bili u cache
            // ta lista nam sluzi da evidentiramo sve ids koji su u cache
            // Pakujemo u cache sve informacije koje smo dobili od klijenta
            foreach (SymbolInfo symbol in infoFromClientForMissingIds)
            {
                _keys.cacheKeys.Add(symbol.Id);
                _cache.Set(symbol.Id, symbol, TimeSpan.FromSeconds(60));
            }

            return infoFromClientForMissingIds;
        }
    }
}
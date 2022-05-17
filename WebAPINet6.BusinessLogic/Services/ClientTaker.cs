using Microsoft.Extensions.Caching.Memory;
using WebAPINet6.BusinessLogic.Model;
using WebAPINet6.WebApi;
using WebAPINet6.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace WebAPINet6.BusinessLogic.Services 
{
     public class ClientTaker : IClientTaker
    {
        private readonly IMemoryCache _cache;
        private readonly IClient _symbol;
        private readonly Keys _keys;
        private readonly ILogger<ClientTaker> _logger;

        public ClientTaker(IMemoryCache cache, IClient symbol, Keys keys, ILogger<ClientTaker> logger)
        {
            _cache = cache;
            _symbol = symbol;
            _keys = keys;
            _logger = logger;
        }

        public async Task<List<SymbolInfo>> GetSymbolsInfo()
        {
            // pozivamo klijenta da nam vrati sve informacije za ids koji nisu u cache
            var infoFromClientForMissingIds = await _symbol.GetSymbols(string.Join(" ", _keys.missingKeys));

            await Task.Run(() =>
            {
                // resetujemo missing keys
                _keys.missingKeys.Clear();

                // U globalnu staticku listu dodajemo sve ids koji nisu bili u cache
                // ta lista nam sluzi da evidentiramo sve ids koji su u cache
                // Pakujemo u cache sve informacije koje smo dobili od klijenta
                foreach (SymbolInfo symbol in infoFromClientForMissingIds)
                {
                    string id = symbol.Id!;
                    _keys.cacheKeys.Add(id!);
                    _cache.Set(symbol.Id, symbol, TimeSpan.FromSeconds(60));

                    _logger.LogInformation("In cache added id: {id}", id);
                }
            });

            return infoFromClientForMissingIds;
        }
    }
}
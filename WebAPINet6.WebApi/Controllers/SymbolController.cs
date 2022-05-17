using Microsoft.AspNetCore.Mvc;
using WebAPINet6.BusinessLogic.Model;
using WebAPINet6.BusinessLogic.Services.Interfaces;

namespace WebAPINet6.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class SymbolController : ControllerBase
    {
        private readonly ICacheTaker _fromCache;
        private readonly IClientTaker _fromClient;
        private readonly Keys _keys;

        public SymbolController( ICacheTaker fromCache, IClientTaker fromClient, Keys keys)
        {
            
            _fromCache = fromCache;
            _fromClient = fromClient;
            _keys = keys;
        }

        [HttpGet("{ids}")]
        public async Task<List<SymbolInfo>> GetSymbols(string ids = "tts-78738373") // tts-78738373 -> Gold
        {    
            // splitujemo sve id-ijeve u slucaju da prosledimo > 1 id-a u request,
            // npr: https://localhost:7050/Symbol/GetSymbols/tts-78738373&tts-78738433
            string[] arrayIds = ids.Split(" ");

            var symbolsInfoFromCache = await _fromCache.GetSymbolsInfo(arrayIds);

            var symbolsInfoFromClient = new List<SymbolInfo>();
            if (_keys.missingKeys.Any()) 
            {
                symbolsInfoFromClient = await _fromClient.GetSymbolsInfo();
            }

            return symbolsInfoFromCache.Concat(symbolsInfoFromClient).ToList();
        }
    }
}

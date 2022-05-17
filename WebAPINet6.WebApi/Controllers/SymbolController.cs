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
        private readonly ILogger<SymbolController> _logger;

        public SymbolController( ICacheTaker fromCache, IClientTaker fromClient, Keys keys, ILogger<SymbolController> logger)
        {
            
            _fromCache = fromCache;
            _fromClient = fromClient;
            _keys = keys;
            _logger = logger;
        }

        [HttpGet("{ids}")]
        public async Task<List<SymbolInfo>> GetSymbols(string ids = "tts-78738373") // tts-78738373 -> Gold
        {    
            // splitujemo sve id-ijeve u slucaju da prosledimo > 1 id-a u request,
            // npr: https://localhost:7050/Symbol/GetSymbols/tts-78738373&tts-78738433
            string[] arrayIds = ids.Split(" ");

            _logger.LogInformation("Input je ids: {ids}", ids);

            var symbolsInfoFromCache = await _fromCache.GetSymbolsInfo(arrayIds);

            var symbolsInfoFromClient = new List<SymbolInfo>();
            if (_keys.missingKeys.Any()) 
            {
                symbolsInfoFromClient = await _fromClient.GetSymbolsInfo();
            }

            if (symbolsInfoFromCache == null || symbolsInfoFromClient == null)
            {
                _logger.LogWarning("TTWS server ili cache je vratio null za id: {id}", ids);
                return NotFound();
            }
            var result = symbolsInfoFromCache.Concat(symbolsInfoFromClient).ToList();

            _logger.LogInformation("Result je {result}", result);
            return result;
        }
    }
}

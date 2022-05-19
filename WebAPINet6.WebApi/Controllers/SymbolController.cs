using Microsoft.AspNetCore.Mvc;
using WebAPINet6.BusinessLogic.Services.Interfaces;

namespace WebAPINet6.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class SymbolController : ControllerBase
    {
        private readonly IDataTaker _dataTaker;
        private readonly ILogger<SymbolController> _logger;

        public SymbolController(IDataTaker dataTaker, ILogger<SymbolController> logger)
        {
            _dataTaker = dataTaker;
            _logger = logger;
        }

        [HttpGet("{ids}")]
        public async Task<List<SymbolInfo>> GetSymbols(string ids = "tts-78738373") // tts-78738373 -> Gold
        {
            _logger.LogInformation("Input is ids: {ids}", ids);

            // splitujemo sve id-ijeve u slucaju da prosledimo > 1 id-a u requestu,
            // npr: https://localhost:7050/Symbol/GetSymbols/tts-78738373&tts-78738433
            string[] arrayIds = ids.Split(" ");
            var result = await _dataTaker.GetSymbolsInfo(arrayIds);

            _logger.LogInformation("Result count is {count}", result.Count);

            return result;
        }
    }
}

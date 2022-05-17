using WebAPINet6.WebApi;

namespace WebAPINet6.BusinessLogic.Services.Interfaces
{
    public interface IClient
    {
        public Task<List<SymbolInfo>> GetSymbols(string ids);
    }
}

using WebAPINet6.WebApi;

namespace WebAPINet6.BusinessLogic.Services
{
    public interface IXmlParser
    {
        public Task<List<SymbolInfo>> Parse(string xml_string);
    }
}

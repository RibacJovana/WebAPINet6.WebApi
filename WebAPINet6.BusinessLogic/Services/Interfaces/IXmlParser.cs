using WebAPINet6.WebApi;

namespace WebAPINet6.BusinessLogic.Services.Interfaces
{
    public interface IXmlParser
    {
        public List<SymbolInfo> Parse(string xml_string);
    }
}

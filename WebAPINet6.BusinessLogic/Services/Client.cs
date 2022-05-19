using Microsoft.Extensions.Options;
using WebAPINet6.WebApi;
using WebAPINet6.BusinessLogic.Enumerations;
using WebAPINet6.BusinessLogic.Services.Interfaces;

namespace WebAPINet6.BusinessLogic.Services
{
    public class Client : IClient
    {
        private readonly Repository.IClient _client;
        private readonly TTWSConfiguration _ttwsConfig;
        private readonly IXmlParser _parseXml;

        public Client(Repository.IClient client, IOptions<TTWSConfiguration> config, IXmlParser parseXml)
        {
            _client = client;
            _ttwsConfig = config.Value;
            _parseXml = parseXml;
        }

        public async Task<List<SymbolInfo>> GetSymbols(string ids)
        {
            string uri = _ttwsConfig.Uri;
            int customerID = _ttwsConfig.CustomerID;

            string xml_string = await _client.GetSymbolsByIDs(ids, uri, customerID);
            var xml_parse = _parseXml.Parse(xml_string);

            return xml_parse;
        }
    }
}
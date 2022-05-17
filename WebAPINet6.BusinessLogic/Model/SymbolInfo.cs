using System.Xml.Serialization;
using WebAPINet6.BusinessLogic.Model;

namespace WebAPINet6.WebApi
{
    [XmlRoot(ElementName = "Symbol", Namespace = "urn:schemas-teletrader-com:mb", DataType = "string", IsNullable = true)]
    public class SymbolInfo
    {
        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;
        
        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("isin")]
        public string? Isin { get; set; }

        [XmlAttribute("tickerSymbol")]
        public string? TickerSymbol { get; set; }

        [XmlElement("Quote")]
        public SymbolQuote? Quote { get; set; }
    }
}

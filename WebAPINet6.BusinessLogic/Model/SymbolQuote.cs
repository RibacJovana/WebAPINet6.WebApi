using System.Xml.Serialization;

namespace WebAPINet6.BusinessLogic.Model
{
    public class SymbolQuote
    {
        [XmlAttribute("last")]
        public string? Last { get; set; }
        
        [XmlAttribute("open")]
        public string? Open { get; set; }
        
        [XmlAttribute("high")]
        public string? High { get; set; }
        
        [XmlAttribute("low")]
        public string? Low { get; set; }
        
        [XmlAttribute("ask")]
        public string? Ask { get; set; }
        
        [XmlAttribute("tradeVolume")]
        public string? TradeVolume { get; set; }
        
        [XmlAttribute("volume")]
        public string? Volume { get; set; }
    }
}

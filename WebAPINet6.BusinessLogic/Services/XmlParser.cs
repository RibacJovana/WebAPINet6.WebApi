using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WebAPINet6.WebApi;
using WebAPINet6.BusinessLogic.Services.Interfaces;

namespace WebAPINet6.BusinessLogic.Services
{
    public class XmlParser : IXmlParser
    {
        public List<SymbolInfo> Parse(string xml_string)
        {
            List<SymbolInfo>? result = new();

            // dobijamo xml koji je tipa string i parsiramo ga da dobijemo XElement
            XElement _xElement = XElement.Parse(xml_string);
            XNamespace nameSpace = _xElement.Name.Namespace;

            IEnumerable<XElement> symbolsFromXElement = _xElement.Descendants(nameSpace + "Symbol");

            XmlSerializer serializer = new(typeof(SymbolInfo));

            // deserializujemo objekte iz xml formata u objekat klase i dodajemo u nasu listu objekata
            foreach (XElement symbol in symbolsFromXElement)
            {
                using XmlReader reader = symbol.CreateReader();
                result.Add((SymbolInfo)serializer.Deserialize(reader)!);
            }
            return result;
        }
    }
}

/* ------------------> DRUGA RESENJA: <------------------
 
 // => PRVO resenje: ( neefikasno jer se nista ne razume )

    IEnumerable<SymbolInfo> lista_kreiranih_symbolInfo = 
        from s in xml.Descendants(nameSpace + "Symbol")
        select new SymbolInfo{
            Name = s.Attribute("name").Value, Id = s.Attribute("id").Value, Isin = s.Attribute("isin").Value, TickerSymbol = s.Attribute("tickerSymbol").Value
        };
  
  
 // => DRUGO resenje: (neefikasno jer se dva puta prolazi kroz loop )

    var lista_atributa = xml
                    .Descendants(nameSpace + "Symbol")
                    .Select(s => 
                       (name: s.Attribute("name"), id: s.Attribute("id"), isin: s.Attribute("isin"), tickerSymbol: s.Attribute("tickerSymbol")));

    foreach (var el in lista_atributa)
    {
        _listaSymbolInfo.Add(
            new SymbolInfo((string)el.name, (string)el.id, (string)el.isin, (string)el.tickerSymbol)
        );
    }
*/

/* ----------> SERIALIZE RESENJE NEBOJSA: <----------
 
    XDocument doc = XDocument.Parse(xml);
    XNamespace ns = doc.Root.Name.Namespace;
    IEnumerable<XElement> xelements = doc.Descendants(ns + "Symbol");
    MemoryStream stream = null;
    List<Symbol> symbols = new List<Symbol>();

    var serializer = new XmlSerializer(typeof(Symbol), "");
    foreach (XElement xelement in xelements)
    {
        stream = new MemoryStream(Encoding.UTF8.GetBytes(xelement.ToString()));

        Symbol symbol = (Symbol)serializer.Deserialize(stream);
        symbols.Add(symbol);
    }
 */

// OVO SAD NE TREBA jer u controlleru imamo: [Produces("application/json")]
// konverujemo objekat u Json,
// Formatting.Indented -> da bude lepo formatirano a ne sve u jednoj liniji
//var result = await _jsonConvert.Convert(symbolsFromXElement, showQuote);
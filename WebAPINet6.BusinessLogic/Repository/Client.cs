using Polly;
using Polly.Retry;
using System.Net.Http;

namespace WebAPINet6.BusinessLogic.Repository
{
    public class Client : IClient
    {
        private readonly IHttpClientFactory _clientFactory;

        public Client(IHttpClientFactory client)
        {
            _clientFactory = client;
        }

        public async Task<string> GetSymbolsByIDs(string ids, string uri, int customerID)
        {
            HttpClient client = _clientFactory.CreateClient("Client");
            HttpResponseMessage response = new();

            response = await client.GetAsync($"{uri}/ttws-net/?action=getSymbols&customerID={customerID}&id={ids}", HttpCompletionOption.ResponseContentRead);
            if (response.Headers.GetValues("errorNumber").FirstOrDefault() != "0")
                response.StatusCode = (System.Net.HttpStatusCode)500;
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}

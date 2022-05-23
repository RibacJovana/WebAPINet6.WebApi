using System.Net;

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

            HttpResponseMessage response = await client.GetAsync($"{uri}/ttws-net/?action=getSymbols&customerID={customerID}&id={ids}", HttpCompletionOption.ResponseContentRead);

            if (response.Headers.GetValues("errorNumber").FirstOrDefault() != "0")
                response.StatusCode = HttpStatusCode.InternalServerError;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}

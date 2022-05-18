using Polly;
using Polly.Retry;

namespace WebAPINet6.BusinessLogic.Repository
{
    public class Client : IClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly AsyncRetryPolicy _policy;

        public Client(IHttpClientFactory client)
        {
            _clientFactory = client;
            _policy = Policy.Handle<Exception>().WaitAndRetryAsync(new[]
            {
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromSeconds(1)
            }, (exception, timespan) =>
            {
                Console.WriteLine(exception.Message, timespan);
            });
        }

        public async Task<string> GetSymbolsByIDs(string ids, string uri, int customerID)
        {
            HttpClient client = _clientFactory.CreateClient("Client");
            HttpResponseMessage? response = null;
            await _policy.ExecuteAsync(async () =>
            {
                response = await client.GetAsync($"{uri}/ttws-net/?action=getSymbols&customerID={customerID}&id={ids}");
                response.EnsureSuccessStatusCode();
            });
           
            return await response!.Content.ReadAsStringAsync();
        }
    }
}

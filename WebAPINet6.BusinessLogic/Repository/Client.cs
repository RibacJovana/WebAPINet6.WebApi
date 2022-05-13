using Polly;
using Polly.Retry;

namespace WebAPINet6.BusinessLogic.Repository
{
    public class Client : IClient
    {
        private readonly HttpClient _client;
        private readonly AsyncRetryPolicy _policy;

        public Client(HttpClient client)
        {
            _client = client;
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
            HttpResponseMessage? response = null;
            await _policy.ExecuteAsync(async () =>
            {
                response = await _client.GetAsync($"{uri}/ttws-net/?action=getSymbols&customerID={customerID}&id={ids}");
                response.EnsureSuccessStatusCode();
            });
           
            return await response.Content.ReadAsStringAsync();
        }
    }
}

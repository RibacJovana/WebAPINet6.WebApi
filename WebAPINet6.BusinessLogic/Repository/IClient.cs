namespace WebAPINet6.BusinessLogic.Repository
{
    public interface IClient
    {
        public Task<string> GetSymbolsByIDs(string ids, string uri, int customerID);
    }
}

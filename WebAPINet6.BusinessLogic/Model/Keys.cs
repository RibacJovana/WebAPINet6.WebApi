namespace WebAPINet6.BusinessLogic.Model
{
    public class Keys
    {
        public HashSet<string> cacheKeys = new();

        public HashSet<string> missingKeys = new();
    }
}

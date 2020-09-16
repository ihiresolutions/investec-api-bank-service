namespace bank_service.integration.contract.Agents
{
    public interface ICache
    {
        string GetCacheString(string cacheKey);
        void SetCacheString(string cacheKey, string value);
    }
}

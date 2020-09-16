using Microsoft.Extensions.Options;

namespace bank_service.integration.contract.Ioc.Options
{
    public class PersistentCacheOptions : IOptions<PersistentCacheOptions>
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public PersistentCacheOptions Value => this;
    }
}

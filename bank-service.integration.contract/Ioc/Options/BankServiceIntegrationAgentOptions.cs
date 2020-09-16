using Microsoft.Extensions.Options;

namespace bank_service.integration.contract.Ioc.Options
{
    public class BankServiceIntegrationAgentOptions : IOptions<BankServiceIntegrationAgentOptions>
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccountNumber { get; set; }
        public BankServiceIntegrationAgentOptions Value => this;
    }
}

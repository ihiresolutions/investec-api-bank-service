using Microsoft.Extensions.Options;
using System;

namespace bank_service.integration.contract.Ioc.Options
{
    public class WalletServiceIntegrationAgentOptions : IOptions<WalletServiceIntegrationAgentOptions>
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public Uri WalletServiceUrl
        {
            get
            {
                return new Uri($"{Host}:{Port}");
            }
        }
        public WalletServiceIntegrationAgentOptions Value => this;
    }
}

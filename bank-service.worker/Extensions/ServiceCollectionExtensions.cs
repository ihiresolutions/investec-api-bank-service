using bank_service.integration.Agents;
using bank_service.integration.contract.Agents;
using bank_service.integration.contract.Ioc.Options;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace bank_service.worker.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistentCache(
            this IServiceCollection services,
            Action<PersistentCacheOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.AddSingleton<ICache, PersistentCache>();

            return services;
        }

        public static IServiceCollection AddBankServiceIntegrationAgent(
            this IServiceCollection services,
            Action<BankServiceIntegrationAgentOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.AddScoped<IBankServiceIntegrationAgent, BankServiceIntegrationAgent>();

            return services;
        }

        public static IServiceCollection AddWalletServiceIntegration(
            this IServiceCollection services,
            Action<WalletServiceIntegrationAgentOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.AddScoped<IOnTransactionFound, WalletServiceOnTransactionFound>();

            return services;
        }
    }
}

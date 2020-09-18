using bank_service.integration.contract.Agents;
using bank_service.worker.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace bank_service.worker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting bank account sync...");

            // Setup DI
            var services = new ServiceCollection()
                .AddHttpClient()
                .AddPersistentCache(options =>
                {
                    options.Host = "localhost";
                    options.Port = 6379;
                    options.Password = "";
                })
                .AddBankServiceIntegrationAgent(options =>
                {
                    options.ClientId = "";
                    options.ClientSecret = "";
                    options.AccountNumber = "";
                })
                .AddWalletServiceIntegration(options =>
                {
                    options.Host = "http://localhost";
                    options.Port = 5000;
                })
                .BuildServiceProvider();

            var bankService = services.GetService<IBankServiceIntegrationAgent>();
            bankService.UpdateAccountTransactions();

            Console.WriteLine("Sync completed!");
        }
    }
}

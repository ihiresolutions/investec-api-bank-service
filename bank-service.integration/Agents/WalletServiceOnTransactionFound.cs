using bank_service.integration.contract.Agents;
using bank_service.integration.contract.Ioc.Options;
using bank_service.integration.contract.Models;
using bank_service.integration.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bank_service.integration.Agents
{
    public class WalletServiceOnTransactionFound : IOnTransactionFound
    {
        #region Constructors

        public WalletServiceOnTransactionFound(IOptions<WalletServiceIntegrationAgentOptions> options, 
            IHttpClientFactory httpClientFactory)
        {
            _options = options;
            _httpClient = httpClientFactory.CreateClient();
        }

        #endregion

        public void PerformUpdate(Transaction transaction)
        {
            // Check if wallet exists
            var responseCode = GetWalletByReference(transaction.Description).Result;
            if (responseCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("Wallet with reference [{0}] was not found", transaction.Description);
                return;
            }
            if (responseCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Unexpected Http Response: [{0}]", responseCode);
                throw new InvalidOperationException("Cannot update wallets");
            }
            // Call the update method
            Console.WriteLine("Wallet [{0}] exists. I will now [{1}] wallet with [{2}] coins", transaction.Description, transaction.TransactionType, transaction.Amount);
            switch(transaction.TransactionType)
            {
                case TransactionType.Credit:
                    {
                        var transactionResponseCode = AddWalletCreditTransaction(transaction, transaction.Description).Result;
                        if (transactionResponseCode != HttpStatusCode.Created)
                        {
                            Console.WriteLine("Error ocurred whilst crediting wallet: [{0}]", transactionResponseCode);
                            throw new InvalidOperationException("Cannot update wallets");
                        }
                        Console.WriteLine("Wallet [{0}] has been credited with [{1}] coins successfully!", transaction.Description, transaction.Amount);
                    }
                    break;
                case TransactionType.Debit:
                    Console.WriteLine("Debit wallet not implemented yet");
                    break;
                case TransactionType.None:
                default:
                    Console.WriteLine("Error - Not a valid transaction type");
                    break;
            }
        }

        #region Helper Methods

        private async Task<HttpStatusCode> GetWalletByReference(string reference)
        {
            try
            {
                string url = string.Format("{0}/{1}/{2}", _options.Value.WalletServiceUrl, WALLETS_RESOURCE, reference);
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "application/json");
                var response = await _httpClient.SendAsync(request);
                return response.StatusCode;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error: [{0}]", exception.Message);
                return HttpStatusCode.InternalServerError;
            }
            
        }

        private async Task<HttpStatusCode> AddWalletCreditTransaction(Transaction transaction, string reference)
        {
            try
            {
                var json = new AddWalletTransactionDto
                {
                    Source = "Bank-Service",
                    Type  = AddWalletTransactionDto.TransactionType.Credit,
                    Amount = transaction.Amount,
                    Currency = "ZAR",
                    Trigger = AddWalletTransactionDto.TransactionTrigger.Bank,
                    TransactionDate = transaction.TransactionDate
                };
                string url = string.Format("{0}/{1}/{2}", _options.Value.WalletServiceUrl, CREATE_CREDIT_TRANSACTION_RESOURCE, reference);
                var transactionJson = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, transactionJson);
                return response.StatusCode;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error: [{0}]", exception.Message);
                return HttpStatusCode.InternalServerError;
            }
        }

        #endregion

        #region Fields

        private IOptions<WalletServiceIntegrationAgentOptions> _options;
        private readonly HttpClient _httpClient;
        private const string WALLETS_RESOURCE = "api/wallet";
        private const string CREATE_CREDIT_TRANSACTION_RESOURCE = "api/wallet/credittransaction";

        #endregion
    }
}

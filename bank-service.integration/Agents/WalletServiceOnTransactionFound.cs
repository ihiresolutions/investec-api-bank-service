using bank_service.integration.contract.Agents;
using bank_service.integration.contract.Ioc.Options;
using bank_service.integration.contract.Models;
using bank_service.integration.WalletServiceModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace bank_service.integration.Agents
{
    public class WalletServiceOnTransactionFound : IOnTransactionFound
    {
        #region Constructors

        public WalletServiceOnTransactionFound(IOptions<WalletServiceIntegrationAgentOptions> options)
        {
            _options = options;
        }

        #endregion

        public void PerformUpdate(Transaction transaction)
        {
            using (var httpClient = new WalletServiceHttpClient(_options.Value.WalletServiceUrl.AbsoluteUri))
            {
                Console.WriteLine(
                    string.Format("I will then [{0}] the wallet with the {1} BountyHunter Coins", 
                    transaction.TransactionType, transaction.Amount));
            }
        }

        #region Fields

        private IOptions<WalletServiceIntegrationAgentOptions> _options;

        #endregion

        /// <summary>
        /// TODO - Build SDK for Wallet Service
        /// </summary>
        private class WalletServiceHttpClient : HttpClient
        {
            #region Constructors

            public WalletServiceHttpClient(string walletServiceUrl)
            {
                _walletServiceUrl = walletServiceUrl;
                this.DefaultRequestHeaders.Add("accept", "application/json");
            }

            #endregion

            #region Methods

            internal string GetWalletByReference(string reference)
            {
                return GetStringAsync(string.Format("{0}/{1}/{2}", _walletServiceUrl, WALLETS_RESOURCE, reference))
                    .GetAwaiter().GetResult();
            }

            internal string AddWalletTransaction(AddWalletTransactionRequest walletTransactionRequest, string referenceNumber)
            {
                var data = new StringContent(JsonConvert.SerializeObject(walletTransactionRequest), Encoding.UTF8, "application/json");
                var httpResponse = PostAsync(string.Format("{0}/{1}/{2}", _walletServiceUrl, CREATE_CREDIT_TRANSACTION_RESOURCE, referenceNumber), data)
                    .GetAwaiter().GetResult();
                if (httpResponse.IsSuccessStatusCode)
                {
                    return httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                string error = string.Format("Http CreateCreditWalletTransaction did not return a success code. [HttpStatusCode]: {0}, [ReasonPhrase]: {1}", httpResponse.StatusCode, httpResponse.ReasonPhrase);
                Console.WriteLine(error);
                return string.Empty;
            }

            #endregion

            #region Fields

            private const string WALLETS_RESOURCE = "/api/wallet";
            private const string CREATE_CREDIT_TRANSACTION_RESOURCE = "/api/wallet/credittransaction";
            private readonly string _walletServiceUrl;

            #endregion
        }
    }
}

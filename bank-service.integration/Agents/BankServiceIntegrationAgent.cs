using bank_service.integration.contract.Agents;
using bank_service.integration.contract.Ioc.Options;
using bank_service.integration.Mappers;
using iHire.InvestecApi.SDK;
using iHire.InvestecApi.SDK.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace bank_service.integration.Agents
{
    public class BankServiceIntegrationAgent : IBankServiceIntegrationAgent
    {
        #region Constructors

        public BankServiceIntegrationAgent(IOptions<BankServiceIntegrationAgentOptions> options,
            IOnTransactionFound onTransactionFound, ICache cache)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _investecSDK = new InvestecSDK(new InvestecConfiguration(
                options.Value.ClientId, options.Value.ClientSecret, options.Value.AccountNumber));
            _onTransactionFound = onTransactionFound;
            _cache = cache;
        }

        #endregion

        public void UpdateAccountTransactions()
        {
            Console.WriteLine("Calling InvestecOpenBankingApi for Transactions...");
            var transactions = _investecSDK.GetTransactions();
            if (!transactions.Any())
            {
                Console.WriteLine("No transactions for account");
                return;
            }
            Console.WriteLine("Transaction returned from API...");

            // Hash the transactions returned
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Fetch transaction payload hash from cache to determine if there are newer transactions
                var currentTransactionsHash = GetHash(sha256Hash, JsonConvert.SerializeObject(transactions));
                var existingTransactionsHash = _cache.GetCacheString(COMPLETE_TRANSACTION_PAYLOAD_HASH_KEY);
                if (string.IsNullOrEmpty(existingTransactionsHash))
                {
                    Console.WriteLine("No existing transactions. Using the entire list to update...");
                    UpdateWalletTransactions(transactions);
                    UpdateKnownTransactionHashes(transactions, currentTransactionsHash, sha256Hash);
                    return;
                }
                else if (existingTransactionsHash.Equals(currentTransactionsHash))
                {
                    Console.WriteLine("Transaction payload does not differ from the previous payload. Skipping this run");
                    return;
                }
                else
                {
                    Console.WriteLine("Existing Transaction Hash that differs from the last known run. Checking for differences");
                    var lastKnownTransactionDate = _cache.GetCacheString(LAST_KNOWN_TRANSACTION_DATETIME_KEY);
                    var filteredTransactionList = transactions.Where(x => x.TransactionDate >= DateTime.Parse(lastKnownTransactionDate));
                    var lastKnownTransactionHash = _cache.GetCacheString(LAST_KNOWN_TRANSACTION_HASH_KEY);
                    UpdateWalletTransactions(lastKnownTransactionHash, filteredTransactionList);
                    UpdateKnownTransactionHashes(transactions, currentTransactionsHash, sha256Hash);
                }
            }
            Console.WriteLine("Completed updating of accounts...");
        }

        #region Helper Methods

        private void UpdateKnownTransactionHashes(TransactionDto[] transactions, string currentTransactionsHash, SHA256 sha256)
        {
            Console.WriteLine("Updating last known transaction hashes");
            _cache.SetCacheString(COMPLETE_TRANSACTION_PAYLOAD_HASH_KEY, currentTransactionsHash);
            var lastKnownTransaction = transactions.OrderBy(x => x.TransactionDate).Last();
            _cache.SetCacheString(LAST_KNOWN_TRANSACTION_HASH_KEY, GetHash(sha256, JsonConvert.SerializeObject(lastKnownTransaction)));
            _cache.SetCacheString(LAST_KNOWN_TRANSACTION_DATETIME_KEY, lastKnownTransaction.TransactionDate.ToString());
        }

        private void UpdateWalletTransactions(TransactionDto[] transactions)
        {
            foreach (var transaction in transactions.OrderBy(x => x.TransactionDate))
            {
                _onTransactionFound.PerformUpdate(transaction.Map());
            }
        }

        private void UpdateWalletTransactions(string lastKnownTransactionHash, IEnumerable<TransactionDto> filteredTransactionList)
        {
            foreach (var transaction in filteredTransactionList.OrderBy(x => x.TransactionDate))
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    var transactionHash = GetHash(sha256Hash, JsonConvert.SerializeObject(transaction));
                    if (transactionHash.Equals(lastKnownTransactionHash))
                    {
                        Console.WriteLine("Transaction hash found. Skipping transaction with description: [{0}]", transaction.Description);
                        continue;
                    }
                    else
                    {
                        UpdateWalletTransactions(new TransactionDto[1] { transaction });
                    }
                }
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        #endregion

        #region Fields

        private readonly InvestecSDK _investecSDK;
        private readonly IOnTransactionFound _onTransactionFound;
        private readonly ICache _cache;
        private const string COMPLETE_TRANSACTION_PAYLOAD_HASH_KEY = "COMPLETE_TRANSACTION_PAYLOAD_HASH";
        private const string LAST_KNOWN_TRANSACTION_HASH_KEY = "LAST_KNOWN_TRANSACTION_HASH";
        private const string LAST_KNOWN_TRANSACTION_DATETIME_KEY = "LAST_KNOWN_TRANSACTION_DATETIME";

        #endregion

        #region Helper Classes

        private class InvestecConfiguration : iHire.InvestecApi.SDK.Contracts.IConfiguration
        {
            public InvestecConfiguration(string clientId, string secret, string accountNumber)
            {
                this.ClientId = clientId;
                this.Secret = secret;
                this.AccountNumber = accountNumber;
            }
            public string ClientId { get; private set; }

            public string Secret { get; private set; }

            public string AccountNumber { get; private set; }

            public string BaseUrl
            {
                get
                {
                    return "https://openapi.investec.com/za/pb/v1";
                }
            }
        }

        #endregion
    }
}

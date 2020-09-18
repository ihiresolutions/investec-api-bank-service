using System;

namespace bank_service.integration.Models
{
    public sealed class AddWalletTransactionDto
    {
        public string Source { get; set; }
        public TransactionType Type { get; set; }
        public TransactionTrigger Trigger { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }

        public enum TransactionType
        {
            None = 0,
            Credit = 1,
            Debit = 2
        }

        public enum TransactionTrigger
        {
            None = 0,
            Bank = 1,
            Wallet = 2
        }
    }
}

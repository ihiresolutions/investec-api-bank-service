using System;

namespace bank_service.integration.contract.Models
{
    public class Transaction
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public Currency Currency { get; set; }
    }
    public enum TransactionType
    {
        None = 0,
        Credit,
        Debit
    }

    public enum Currency
    {
        None = 0,
        ZAR
    }
}

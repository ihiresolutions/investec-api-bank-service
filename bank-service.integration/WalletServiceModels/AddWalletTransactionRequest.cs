namespace bank_service.integration.WalletServiceModels
{
    /// <summary>
    /// TODO - Build SDK for Wallet service
    /// </summary>
    public class AddWalletTransactionRequest
    {
        public string Source { get; set; }
        public string Type { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Trigger { get; set; }
    }
}

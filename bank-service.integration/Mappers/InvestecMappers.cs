using bank_service.integration.contract.Models;
using iHire.InvestecApi.SDK.Models;

namespace bank_service.integration.Mappers
{
    public static class InvestecMappers
    {
        public static Transaction Map(this TransactionDto integrationDto)
        {
            return integrationDto == null
                ? null
                : new Transaction
                {
                    Status = integrationDto.Status,
                    Amount = integrationDto.Amount,
                    Description = integrationDto.Description,
                    TransactionDate = integrationDto.TransactionDate,
                    TransactionType = Map(integrationDto.TransactionType),
                    Currency = Map(integrationDto.Currency)
                };
        }

        public static TransactionType Map(iHire.InvestecApi.SDK.Enumerations.TransactionType transactionType)
        {
            switch(transactionType)
            {
                case iHire.InvestecApi.SDK.Enumerations.TransactionType.CREDIT:
                    return TransactionType.Credit;
                case iHire.InvestecApi.SDK.Enumerations.TransactionType.DEBIT:
                    return TransactionType.Debit;
                default:
                    return TransactionType.None;
            }
        }

        public static Currency Map(iHire.InvestecApi.SDK.Enumerations.Currency currency)
        {
            switch(currency)
            {
                case iHire.InvestecApi.SDK.Enumerations.Currency.ZAR:
                    return Currency.ZAR;
                default:
                    return Currency.None;
            }
        }
    }
}

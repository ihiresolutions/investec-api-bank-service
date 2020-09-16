using bank_service.integration.contract.Models;

namespace bank_service.integration.contract.Agents
{
    public interface IOnTransactionFound
    {
        void PerformUpdate(Transaction transaction);
    }
}

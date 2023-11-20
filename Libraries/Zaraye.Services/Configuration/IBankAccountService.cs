using Zaraye.Core;
using Zaraye.Core.Domain.Configuration;
using System.Threading.Tasks;

namespace Zaraye.Services.Configuration
{
    public partial interface IBankAccountService
    {
        Task InsertBankAccountAsync(BankAccount bankAccount);
        Task UpdateBankAccountAsync(BankAccount bankAccount);
        Task DeleteBankAccountAsync(BankAccount bankAccount);
        Task<BankAccount> GetBankAccountByIdAsync(int id);
        Task<IPagedList<BankAccount>> GetAllBankAccountAsync(bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
    }
}

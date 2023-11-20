using Zaraye.Core;
using Zaraye.Core.Domain.Configuration;
using Zaraye.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Configuration
{
    public partial class BankAccountService : IBankAccountService
    {
        #region Fields

        private readonly IRepository<BankAccount> _bankAccountRepository;

        #endregion

        #region Ctor

        public BankAccountService(IRepository<BankAccount> bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        #endregion

        #region Methods

        public virtual async Task InsertBankAccountAsync(BankAccount bankAccount)
        {
            await _bankAccountRepository.InsertAsync(bankAccount);
        }

        public virtual async Task UpdateBankAccountAsync(BankAccount bankAccount)
        {
            await _bankAccountRepository.UpdateAsync(bankAccount);
        }

        public virtual async Task DeleteBankAccountAsync(BankAccount bankAccount)
        {
            await _bankAccountRepository.DeleteAsync(bankAccount);
        }

        public virtual async Task<BankAccount> GetBankAccountByIdAsync(int id)
        {
            return await _bankAccountRepository.GetByIdAsync(id);
        }

        public virtual async Task<IPagedList<BankAccount>> GetAllBankAccountAsync(bool showHidden = false, int pageIndex = 0,
            int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var model = await _bankAccountRepository.GetAllPagedAsync(query =>
            {
                query = query.Where(c => !c.Deleted);

                if (!showHidden)
                    query = query.Where(t => t.Active);

                return query;
            }, pageIndex, pageSize, getOnlyTotalCount);

            return model;
        }

        #endregion
    }
}

using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial class AppFeedBackService : IAppFeedBackService
    {
        #region Fileds

        private readonly IRepository<AppFeedBack> _appFeedBackrepository;
        private readonly IRepository<Customer> _customerrepository;
        private readonly IRepository<UserType> _userTyperepository;
        private readonly IRepository<GenericAttribute> _genricAttributerepository;

        #endregion

        #region Ctor

        public AppFeedBackService(
            IRepository<AppFeedBack> appFeedBackrepository,
            IRepository<Customer> customerrepository,
            IRepository<UserType> userTyperepository,
            IRepository<GenericAttribute> genricAttributerepository
            )
        {
            _appFeedBackrepository = appFeedBackrepository;
            _customerrepository = customerrepository;
            _userTyperepository = userTyperepository;
            _genricAttributerepository = genricAttributerepository;
        }

        #endregion

        #region Mehtod

        public virtual async Task<IPagedList<AppFeedBack>> GetAllAppFeedBackAsync(string searchFeedBack = "",
            string searchOwnerEmail = "", string searchUserEmail = "", string searchFullName = "", 
            string searchUsername = "" , DateTime? searchStartDate = null, DateTime? searchEndDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from f in _appFeedBackrepository.Table
                        join c in _customerrepository.Table on f.UserId equals c.Id
                        where !c.Deleted && 
                        (string.IsNullOrWhiteSpace(searchUserEmail) || c.Email.Contains(searchUserEmail)) && 
                        (string.IsNullOrWhiteSpace(searchFullName) || c.FullName.Contains(searchFullName)) && 
                        (string.IsNullOrWhiteSpace(searchUsername) || c.Username.Contains(searchUsername))
                        orderby f.Id descending
                        select f;

            if (!string.IsNullOrWhiteSpace(searchFeedBack))
                query = query.Where(x => x.FeedBack.Contains(searchFeedBack));

            if (!string.IsNullOrWhiteSpace(searchOwnerEmail))
                query = query.Where(x => x.OwnerEmail.Contains(searchOwnerEmail));

            if (searchStartDate.HasValue)
                query = query.Where(o => searchStartDate.Value <= o.CreatedOnUtc);

            if (searchEndDate.HasValue)
                query = query.Where(o => searchEndDate.Value >= o.CreatedOnUtc);

            var appFeedBacks = await query.ToPagedListAsync(pageIndex, pageSize);
            return appFeedBacks;
        }

        public virtual async Task<AppFeedBack> GetFeedBackByIdAsync(int feedBackId)
        {
            return await _appFeedBackrepository.GetByIdAsync(feedBackId);
        }

        public virtual async Task InsertAppFeedBackAsync(AppFeedBack appFeedBack)
        {
            await _appFeedBackrepository.InsertAsync(appFeedBack);
        }

        public virtual async Task UpdateAppFeedBackAsync(AppFeedBack appFeedBack)
        {
            await _appFeedBackrepository.UpdateAsync(appFeedBack);
        }

        #endregion
    }

}
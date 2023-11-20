using Zaraye.Core;
using Zaraye.Core.Domain.Common;
using Zaraye.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial class BidRequestTrackerService : IBidRequestTrackerService
    {
        #region Fields

        private readonly IRepository<BidRequestTracker> _bidRequestTracker;


        #endregion

        #region Ctor

        public BidRequestTrackerService(IRepository<BidRequestTracker> BidRequestTrackerRepository)
        {
            _bidRequestTracker = BidRequestTrackerRepository;
        }

        #endregion

        #region Methods


        public virtual async Task DeleteBidRequestTrackerAsync(BidRequestTracker bidRequestTracker)
        {
            await _bidRequestTracker.DeleteAsync(bidRequestTracker);
        }

        public virtual async Task InsertBidRequestTrackerAsync(BidRequestTracker bidRequestTracker)
        {
            if (bidRequestTracker == null)
                throw new ArgumentNullException(nameof(bidRequestTracker));

            bidRequestTracker.CreatedOnUtc = DateTime.UtcNow;


            await _bidRequestTracker.InsertAsync(bidRequestTracker);
        }

        public async Task<bool> FindBidRequestTracker(int targetId, string trackerType, int statusId)
        {
            if (targetId == 0 || string.IsNullOrWhiteSpace(trackerType) || statusId == 0)
                return false;

            var bidRequestTracker = await _bidRequestTracker.Table.Where(x => x.TargetId == targetId && x.TrackerType == trackerType && x.StatusId == statusId).
                OrderByDescending(x => x.Id).FirstOrDefaultAsync();

            if (bidRequestTracker is null)
                return false;

            return true;
        }
        #endregion
    }

}
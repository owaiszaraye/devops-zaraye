using Zaraye.Core;
using Zaraye.Core.Domain.Common;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial interface IBidRequestTrackerService
    {
        Task DeleteBidRequestTrackerAsync(BidRequestTracker bidRequestTracker);
        Task InsertBidRequestTrackerAsync(BidRequestTracker bidRequestTracker);
        Task<bool> FindBidRequestTracker(int targetId, string trackerType, int statusId);
    }
}
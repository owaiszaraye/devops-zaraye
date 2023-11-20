using Zaraye.Core;
using Zaraye.Core.Domain.Common;
using System.Threading.Tasks;
using System;

namespace Zaraye.Services.Common
{
    public partial interface IFaqService
    {
        Task InsertFaqAsync(Faq faq);
        Task UpdateFaqAsync(Faq faq);
        Task<Faq> GetFaqByIdAsync(int faqId);
        Task DeleteFaqAsync(Faq faq);
        Task<IPagedList<Faq>> GetAllFaqAsync(string searchQuestion = "", string searchAnswer = "", bool? overridePublished = null,
             bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<IPagedList<Faq>> GetAllFaqbyTypeAsync(bool? type = true, string searchQuestion = "", string searchAnswer = "", bool? overridePublished = null,
             bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
using Zaraye.Core;
using Zaraye.Core.Domain.Common;
using Zaraye.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Zaraye.Services.Common
{
    public partial class FaqService : IFaqService
    {
        #region Fileds
        private readonly IRepository<Faq> _faqRepository;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public FaqService(IRepository<Faq> faqRepository,
            IWorkContext workContext)
        {
            _faqRepository = faqRepository;
            _workContext = workContext;
        }
        #endregion

        #region Mehtod

        public virtual async Task InsertFaqAsync(Faq faq)
        {
            await _faqRepository.InsertAsync(faq);
        }

        public virtual async Task UpdateFaqAsync(Faq faq)
        {
            await _faqRepository.UpdateAsync(faq);
        }

        public virtual async Task<Faq> GetFaqByIdAsync(int faqId)
        {
            return await _faqRepository.GetByIdAsync(faqId);
        }

        public virtual async Task DeleteFaqAsync(Faq faq)
        {
            await _faqRepository.DeleteAsync(faq);
        }

        public virtual async Task<IPagedList<Faq>> GetAllFaqAsync(string searchQuestion = "", string searchAnswer = "", bool? overridePublished = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from f in _faqRepository.Table
                        where !f.Deleted
                        select f;

            if (!string.IsNullOrWhiteSpace(searchQuestion))
                query = query.Where(x => x.Question.Contains(searchQuestion));

            if (!string.IsNullOrWhiteSpace(searchAnswer))
                query = query.Where(x => x.Answer.Contains(searchAnswer));

            if (!showHidden)
                query = query.Where(br => br.Published);
            else if (overridePublished.HasValue)
                query = query.Where(c => c.Published == overridePublished.Value);

            var faqs = await query.ToPagedListAsync(pageIndex, pageSize);
            return faqs;
        }

        public virtual async Task<IPagedList<Faq>> GetAllFaqbyTypeAsync(bool? type = true, string searchQuestion = "", string searchAnswer = "", bool? overridePublished = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from f in _faqRepository.Table
                        where !f.Deleted
                        select f;

            if (type == true)
                type = false;
            else
                type = true;

            query = query.Where(x => x.Type == type);

            if (!string.IsNullOrWhiteSpace(searchQuestion))
                query = query.Where(x => x.Question.Contains(searchQuestion));

            if (!string.IsNullOrWhiteSpace(searchAnswer))
                query = query.Where(x => x.Answer.Contains(searchAnswer));

            if (!showHidden)
                query = query.Where(br => br.Published);
            else if (overridePublished.HasValue)
                query = query.Where(c => c.Published == overridePublished.Value);

            var faqs = await query.ToPagedListAsync(pageIndex, pageSize);
            return faqs;
        }
        #endregion
    }

}
using Zaraye.Core;
using Zaraye.Core.Domain.Common;
using Zaraye.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial class JaizaService : IJaizaService
    {
        #region Fileds
        private readonly IRepository<Jaiza> _jaizaRepository;
        #endregion

        #region Ctor
        public JaizaService(IRepository<Jaiza> jaizaRepository)
        {
            _jaizaRepository = jaizaRepository;
        }
        #endregion

        #region Mehtod

        public virtual async Task InsertJaizaAsync(Jaiza jaiza)
        {
            await _jaizaRepository.InsertAsync(jaiza);
        }

        public virtual async Task UpdateJaizaAsync(Jaiza jaiza)
        {
            await _jaizaRepository.UpdateAsync(jaiza);
        }

        public virtual async Task<Jaiza> GetJaizaByIdAsync(int jaizaId)
        {
            return await _jaizaRepository.GetByIdAsync(jaizaId);
        }

        public virtual async Task<IPagedList<Jaiza>> GetAllJaizaAsync(string searchPrediction = "", string searchRecommendation = "",
            DateTime? searchStartDate = null, DateTime? searchEndDate = null, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _jaizaRepository.Table;

            if (!string.IsNullOrWhiteSpace(searchPrediction))
                query = query.Where(x => x.Prediction.Contains(searchPrediction));

            if (!string.IsNullOrWhiteSpace(searchRecommendation))
                query = query.Where(x => x.Recommendation.Contains(searchRecommendation));

            if (!showHidden)
                query = query.Where(br => br.Published);

            if (searchStartDate.HasValue)
                query = query.Where(br => br.CreatedOnUtc >= searchStartDate.Value);

            if (searchEndDate.HasValue)
                query = query.Where(br => br.CreatedOnUtc <= searchEndDate.Value);

            var faqs = await query.ToPagedListAsync(pageIndex, pageSize);
            return faqs;
        }

        #endregion
    }

}
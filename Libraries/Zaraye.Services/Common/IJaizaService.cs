using Zaraye.Core;
using Zaraye.Core.Domain.Common;
using System;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial interface IJaizaService
    {
        Task InsertJaizaAsync(Jaiza jaiza);

        Task UpdateJaizaAsync(Jaiza jaiza);

        Task<Jaiza> GetJaizaByIdAsync(int jaizaId);

        Task<IPagedList<Jaiza>> GetAllJaizaAsync(string searchPrediction = "", string searchRecommendation = "", DateTime? searchStartDate = null, DateTime? searchEndDate = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);     
    }
}
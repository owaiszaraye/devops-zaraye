using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;
using Zaraye.Core;
using Zaraye.Data;

namespace Zaraye.Services.Common
{
    public partial class WebSliderService : IWebSliderService
    {
        #region Fileds
        private readonly IRepository<WebSlider> _webSliderRepository;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public WebSliderService(IRepository<WebSlider> webSliderRepository,
            IWorkContext workContext)
        {
            _webSliderRepository = webSliderRepository;
            _workContext = workContext;
        }
        #endregion

        #region Mehtod

        public virtual async Task InsertWebSliderAsync(WebSlider webSlider)
        {
            await _webSliderRepository.InsertAsync(webSlider);
        }

        public virtual async Task UpdateWebSliderAsync(WebSlider webSlider)
        {
            await _webSliderRepository.UpdateAsync(webSlider);
        }

        public virtual async Task<WebSlider> GetWebSliderByIdAsync(int webSliderId)
        {
            return await _webSliderRepository.GetByIdAsync(webSliderId);
        }

        public virtual async Task DeleteWebSliderAsync(WebSlider webSlider)
        {
            await _webSliderRepository.DeleteAsync(webSlider);
        }

        public virtual async Task<IPagedList<WebSlider>> GetAllWebSliderAsync(string searchTitle = "",
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from f in _webSliderRepository.Table
                        where !f.Deleted
                        select f;

            if (!string.IsNullOrWhiteSpace(searchTitle))
                query = query.Where(x => x.Title.Contains(searchTitle));


            if (!showHidden)
                query = query.Where(br => br.Published);

            var webSliders = await query.ToPagedListAsync(pageIndex, pageSize);
            return webSliders;
        }

        #endregion
    }
}

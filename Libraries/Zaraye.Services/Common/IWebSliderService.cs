using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;
using Zaraye.Core;

namespace Zaraye.Services.Common
{
    public partial interface IWebSliderService
    {
        Task InsertWebSliderAsync(WebSlider webSlider);

        Task UpdateWebSliderAsync(WebSlider webSlider);

        Task<WebSlider> GetWebSliderByIdAsync(int webSliderId);

        Task DeleteWebSliderAsync(WebSlider webSlider);

        Task<IPagedList<WebSlider>> GetAllWebSliderAsync(string searchTitle = "",
             bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}

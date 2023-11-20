using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Seo;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class TopicTag : BaseEntity, StoreEntity, ILocalizedEntity, ISlugSupported
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        public int StoreId { get; set; }
    }
}
